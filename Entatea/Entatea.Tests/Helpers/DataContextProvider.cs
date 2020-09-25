﻿using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Entatea.InMemory;
using Entatea.MySql;
using Entatea.Resolvers;
using Entatea.SqlServer;

namespace Entatea.Tests.Helpers
{
    public class DataContextProvider
    {
        public static IDataContext SetupDataContext(
            Type dataContextType,
            ITableNameResolver tableNameResolver = null,
            IColumnNameResolver columnNameResolver = null)
        {
            // check whether we are dealing with a type that implement IDataContext
            if (typeof(IDataContext).IsAssignableFrom(dataContextType) == false)
            {
                throw new ArgumentException($"Type {dataContextType.Name} does not implement IDataContext.");
            }

            // return the data context for the given type
            if (typeof(InMemoryDataContext).IsAssignableFrom(dataContextType))
            {
                return new InMemoryDataContext();
            }
            else if (typeof(SqlServerDataContext).IsAssignableFrom(dataContextType))
            {
                LocalDbTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
                return new SqlServerDataContext(
                    LocalDbTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName),
                    tableNameResolver ?? new DefaultTableNameResolver(),
                    columnNameResolver ?? new DefaultColumnNameResolver());
            }
            else if (typeof(MySqlDataContext).IsAssignableFrom(dataContextType))
            {
                MySqlTestHelper.CreateTestDatabase(TestContext.CurrentContext.Test.FullName);
                return new MySqlDataContext(
                    MySqlTestHelper.GetTestConnectionString(TestContext.CurrentContext.Test.FullName),
                    tableNameResolver ?? new DefaultTableNameResolver(),
                    columnNameResolver ?? new DefaultColumnNameResolver());
            }
            else
            {
                throw new ArgumentException($"Type {dataContextType} is not supported, add support in {nameof(Helpers.DataContextProvider)}.cs");
            }
        }

        public static void DeleteDataContext(Type dataContextType)
        {
            // check whether we are dealing with a type that we need to dispose of
            if (typeof(SqlServerDataContext).IsAssignableFrom(dataContextType))
            {
                LocalDbTestHelper.DeleteTestDatabase(TestContext.CurrentContext.Test.FullName);
            } 
            else if (typeof(MySqlDataContext).IsAssignableFrom(dataContextType))
            {
                MySqlTestHelper.DeleteTestDatabase(TestContext.CurrentContext.Test.FullName);
            }
        }
    }
}