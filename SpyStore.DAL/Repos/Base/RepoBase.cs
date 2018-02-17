using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using SpyStore.DAL.EF;
using SpyStore.Models.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpyStore.DAL.Repos.Base
{
    public abstract class RepoBase<T> : IDisposable, IRepo<T> where T : EntityBase, new()
    {
        protected readonly StoreContext Db;
        bool _disposed = false;

        #region Constructors

        protected RepoBase()
        {
            Db = new StoreContext();
            Table = Db.Set<T>();
        }

        protected RepoBase(DbContextOptions<StoreContext> options)
        {
            Db = new StoreContext(options);
            Table = Db.Set<T>();
        }

        #endregion

        public int Count => throw new NotImplementedException();

        public bool HasChanges => throw new NotImplementedException();

        #region Select

        public T Find(int? id) => Table.Find(id);

        public virtual IEnumerable<T> GetAll() => Table;

        public T GetFirst() => Table.FirstOrDefault();

        internal IEnumerable<T> GetRange(IQueryable<T> query, int skip, int take) => query.Skip(skip).Take(take);
        
        public virtual IEnumerable<T> GetRange(int skip, int take) => GetRange(Table,skip, take);

        #endregion

        #region Add

        public int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }

        public int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges() : 0;
        }

        #endregion

        #region Update

        public int Update(T entity, bool persist = true)
        {
            Table.Update(entity);
            return persist ? SaveChanges() : 0;
        }

        public int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? SaveChanges() : 0;
        }

        #endregion

        #region Delete

        public int Delete(T entity, bool persist = true)
        {
            Table.Remove(entity);

            return persist ? SaveChanges() : 0;
        }

        public int Delete(int id, byte[] timeStamp, bool persist = true)
        {
            // Get tracked item if being tracked

            var entry = GetEntryFromChangeTracker(id);
            if (entry != null)
            {
                // if timestamp matches current, then it can be deleted
                if (entry.TimeStamp == timeStamp)
                {
                    return Delete(entry, persist);
                }
                throw new Exception("Unable to delete due to concurrency violation.");
            }
            // if not tracked create entry and mark as deleted
            Db.Entry(new T { Id = id, TimeStamp = timeStamp }).State = EntityState.Deleted;

            return persist ? Db.SaveChanges() : 0;

        }

        public int DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.RemoveRange(entities);

            return persist ? SaveChanges() : 0;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free stuff
            }

            Db.Dispose();
            _disposed = true;
        }

        #endregion

        internal T GetEntryFromChangeTracker(int? id)
        {
            return Db.ChangeTracker.Entries<T>().Select((EntityEntry e) => (T)e.Entity)
                .FirstOrDefault(x => x.Id == id);
        }

        protected DbSet<T> Table;
        public StoreContext Context => Db;

        public int SaveChanges()
        {
            try
            {
                return Db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // A concurrency error occurred
                // Should handle intelligently
                Console.WriteLine(ex);
                throw;
            }
            catch (RetryLimitExceededException ex)
            {
                // DbResiliency retry limit exceeded
                // Should handle intelligently
                Console.WriteLine(ex);
                throw;
            }
            catch (Exception ex)
            {
                // Should handle intelligently
                Console.WriteLine(ex);
                throw;
            }

        }
    }
}
