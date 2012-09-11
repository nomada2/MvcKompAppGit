﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MvcKompApp.DAL.DataAccess
{
    public interface IRepository<TAggregateRoot>
         where TAggregateRoot : class, IAggregateRoot
    {
        void Save(TAggregateRoot instance);

        void Remove(TAggregateRoot instance);

        TAggregateRoot One(Expression<Func<TAggregateRoot, bool>> predicate = null,
                           params Expression<Func<TAggregateRoot, object>>[] includes);

        IQueryable<TAggregateRoot> All(Expression<Func<TAggregateRoot, bool>> predicate = null,
                                       params Expression<Func<TAggregateRoot, object>>[] includes);

        bool Exists(Expression<Func<TAggregateRoot, bool>> predicate = null);

        int Count(Expression<Func<TAggregateRoot, bool>> predicate = null);
    }
}