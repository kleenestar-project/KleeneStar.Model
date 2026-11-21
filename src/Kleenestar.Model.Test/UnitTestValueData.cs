using KleeneStar.Model.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KleeneStar.Model.Test
{
    /// <summary>
    /// Runs the bulk value read of <see cref="ModelHub.GetValueData(IEnumerable{Guid}, IEnumerable{Guid}, KleeneStarDbContext)"/>
    /// against a real SQLite database, since the in-memory provider of the other tests translates
    /// no query.
    /// </summary>
    public class UnitTestValueData : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly Guid _workspaceId = Guid.NewGuid();
        private readonly Guid _classId = Guid.NewGuid();
        private readonly Guid _statusFieldId = Guid.NewGuid();
        private readonly Guid _priorityFieldId = Guid.NewGuid();
        private readonly Guid _otherFieldId = Guid.NewGuid();
        private readonly List<Guid> _objectIds = [];

        /// <summary>
        /// The number of objects, more than one chunk of the bulk read.
        /// </summary>
        private const int ObjectCount = 1100;

        /// <summary>
        /// Opens a database with one class, three fields and objects that each carry a value
        /// for every field.
        /// </summary>
        public UnitTestValueData()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            using var db = Context();

            db.Database.EnsureCreated();
            db.Workspaces.Add(new Workspace { Id = _workspaceId, Key = "VAL", Name = "Values" });
            db.Classes.Add(new Class { Id = _classId, Name = "Ticket", WorkspaceId = _workspaceId });
            db.Fields.Add(new Field { Id = _statusFieldId, Name = "Status", ClassId = _classId });
            db.Fields.Add(new Field { Id = _priorityFieldId, Name = "Priority", ClassId = _classId });
            db.Fields.Add(new Field { Id = _otherFieldId, Name = "Other", ClassId = _classId });

            for (var i = 0; i < ObjectCount; i++)
            {
                var objectId = Guid.NewGuid();
                _objectIds.Add(objectId);

                db.Objects.Add(new Entities.Object(objectId) { Key = $"VAL-{i + 1}", Summary = $"Object {i + 1}", WorkspaceId = _workspaceId, ClassId = _classId });
                db.Values.Add(new Value { Id = Guid.NewGuid(), ObjectId = objectId, FieldId = _statusFieldId, Data = $"status {i}" });
                db.Values.Add(new Value { Id = Guid.NewGuid(), ObjectId = objectId, FieldId = _priorityFieldId, Data = $"priority {i}" });
                db.Values.Add(new Value { Id = Guid.NewGuid(), ObjectId = objectId, FieldId = _otherFieldId, Data = $"other {i}" });
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Creates a context on the shared connection.
        /// </summary>
        /// <returns>The context.</returns>
        private KleeneStarDbContext Context()
        {
            return new KleeneStarDbContext(new DbContextOptionsBuilder<KleeneStarDbContext>().UseSqlite(_connection).Options);
        }

        /// <summary>
        /// Closes the database.
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The read answers the requested fields of every requested object, across chunks, and
        /// nothing of a field that was not requested.
        /// </summary>
        [Fact]
        public void AnswersRequestedFieldsOfEveryObject()
        {
            using var db = Context();

            var result = ModelHub.GetValueData(_objectIds, [_statusFieldId, _priorityFieldId], db);

            Assert.Equal(ObjectCount * 2, result.Count);
            Assert.DoesNotContain(result, x => x.FieldId == _otherFieldId);
            Assert.Contains(result, x => x.ObjectId == _objectIds[^1] && x.FieldId == _priorityFieldId && x.Data == $"priority {ObjectCount - 1}");
        }

        /// <summary>
        /// The read is narrowed to the requested objects.
        /// </summary>
        [Fact]
        public void AnswersOnlyRequestedObjects()
        {
            using var db = Context();

            var result = ModelHub.GetValueData([_objectIds[3]], [_statusFieldId], db);

            Assert.Single(result);
            Assert.Equal("status 3", result[0].Data);
        }

        /// <summary>
        /// No object or no field reads nothing.
        /// </summary>
        [Fact]
        public void EmptyRequestReadsNothing()
        {
            using var db = Context();

            Assert.Empty(ModelHub.GetValueData([], [_statusFieldId], db));
            Assert.Empty(ModelHub.GetValueData(_objectIds, [], db));
        }
    }
}
