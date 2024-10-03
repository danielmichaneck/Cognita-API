using System;
using System.Linq.Expressions;
using Cognita_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cognita_Domain.Repositories;

public class RepositoryBase<T>
    where T : class
{
    protected DbSet<T> DbSet { get; }
    protected CognitaDbContext Context { get; }

    public RepositoryBase(CognitaDbContext context)
    {
        DbSet = context.Set<T>();
        Context = context;
    }

    public async Task CreateAsync(T entity) => await DbSet.AddAsync(entity);

    public void Delete(T entity) => DbSet.Remove(entity);

    public IQueryable<T> GetAll() => DbSet.AsNoTracking();

    public IQueryable<T> GetByCondition(
        Expression<Func<T, bool>> expression,
        bool trackChanges = false
    ) => trackChanges ? DbSet.Where(expression) : DbSet.Where(expression).AsNoTracking();

    public void Update(T entity) => DbSet.Update(entity);
}
