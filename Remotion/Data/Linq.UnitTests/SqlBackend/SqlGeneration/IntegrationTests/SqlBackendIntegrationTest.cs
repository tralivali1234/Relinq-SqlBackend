// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.Linq.Backend.SqlGeneration;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class SqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    public void SimpleSqlQuery_SimpleEntitySelect ()
    {
      CheckQuery (
          from s in Cooks select s,
          "SELECT [t0].[ID],[t0].[FirstName],[t0].[Name],[t0].[IsStarredCook],[t0].[IsFullTimeCook],[t0].[SubstitutedID],[t0].[KitchenID] "
          + "FROM [CookTable] AS [t0]");
    }

    [Test]
    public void SimpleSqlQuery_SimplePropertySelect ()
    {
      CheckQuery (
          from s in Cooks select s.FirstName,
          "SELECT [t0].[FirstName] FROM [CookTable] AS [t0]");
    }

    [Test]
    public void SimpleSqlQuery_EntityPropertySelect ()
    {
      CheckQuery (
          from k in Kitchens select k.Cook,
          "SELECT [t1].[ID],[t1].[FirstName],[t1].[Name],[t1].[IsStarredCook],[t1].[IsFullTimeCook],[t1].[SubstitutedID],[t1].[KitchenID] "
          + "FROM [KitchenTable] AS [t0] JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[KitchenID]");
    }

    [Test]
    public void SimpleSqlQuery_ChainedPropertySelect_EndingWithSimpleProperty ()
    {
      CheckQuery (
          from k in Kitchens select k.Cook.FirstName,
          "SELECT [t1].[FirstName] FROM [KitchenTable] AS [t0] JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[KitchenID]");
    }

    [Test]
    public void SelectQuery_ChainedPropertySelect_WithSameType ()
    {
      CheckQuery (
          from c in Cooks select c.Substitution.FirstName,
          "SELECT [t1].[FirstName] FROM [CookTable] AS [t0] JOIN [CookTable] AS [t1] ON [t0].[ID] = [t1].[SubstitutedID]");
    }

    [Test]
    public void SimpleSqlQuery_ChainedPropertySelect_EndingWithEntityProperty ()
    {
      CheckQuery (
          from k in Kitchens select k.Restaurant.SubKitchen.Cook,
          "SELECT [t3].[ID],[t3].[FirstName],[t3].[Name],[t3].[IsStarredCook],[t3].[IsFullTimeCook],[t3].[SubstitutedID],[t3].[KitchenID] "
          + "FROM [KitchenTable] AS [t0] "
          + "JOIN [RestaurantTable] AS [t1] ON [t0].[RestaurantID] = [t1].[ID] "
          + "JOIN [KitchenTable] AS [t2] ON [t1].[ID] = [t2].[RestaurantID] "
          + "JOIN [CookTable] AS [t3] ON [t2].[ID] = [t3].[KitchenID]");
    }

    [Test]
    [Ignore ("TODO 2407")]
    public void SimpleSqlQuery_ChainedPropertySelectAndWhere_SamePathTwice ()
    {
      CheckQuery (
          from k in Kitchens where k.Restaurant.SubKitchen.Cook != null select k.Restaurant.SubKitchen.Cook,
          "?");
    }

    [Test]
    public void SimpleSqlQuery_ChainedPropertySelectAndWhere_PartialPathTwice ()
    {
      CheckQuery (
          from k in Kitchens where k.Restaurant.SubKitchen.Restaurant.ID == 0 select k.Restaurant.SubKitchen.Cook,
          "SELECT [t4].[ID],[t4].[FirstName],[t4].[Name],[t4].[IsStarredCook],[t4].[IsFullTimeCook],[t4].[SubstitutedID],[t4].[KitchenID] "
          + "FROM [KitchenTable] AS [t0] "
          + "JOIN [RestaurantTable] AS [t1] ON [t0].[RestaurantID] = [t1].[ID] "
          + "JOIN [KitchenTable] AS [t2] ON [t1].[ID] = [t2].[RestaurantID] "
          + "JOIN [RestaurantTable] AS [t3] ON [t2].[RestaurantID] = [t3].[ID] "
          + "JOIN [CookTable] AS [t4] ON [t2].[ID] = [t4].[KitchenID] "
          + "WHERE ([t3].[ID] = @1)",
          new CommandParameter("@1", 0));
    }

    [Test]
    public void SimpleSqlQuery_ConstantSelect ()
    {
      CheckQuery (
          from k in Kitchens select "hugo",
          "SELECT @1 FROM [KitchenTable] AS [t0]",
          new CommandParameter ("@1", "hugo"));
    }

    [Test]
    public void SimpleSqlQuery_NullSelect ()
    {
      CheckQuery (
          Kitchens.Select<Kitchen, object> (k => null),
          "SELECT NULL FROM [KitchenTable] AS [t0]");
    }

    [Test]
    public void SimpleSqlQuery_TrueSelect ()
    {
      CheckQuery (
          from k in Kitchens select true,
          "SELECT @1 FROM [KitchenTable] AS [t0]",
          new CommandParameter ("@1", 1));
    }

    [Test]
    public void SimpleSqlQuery_FalseSelect ()
    {
      CheckQuery (
          from k in Kitchens select false,
          "SELECT @1 FROM [KitchenTable] AS [t0]",
          new CommandParameter ("@1", 0));
    }

    [Test]
    public void SelectQuery_WithWhereCondition ()
    {
      CheckQuery (
          from c in Cooks where c.Name == "Huber" select c.FirstName,
          "SELECT [t0].[FirstName] FROM [CookTable] AS [t0] WHERE ([t0].[Name] = @1)",
          new CommandParameter ("@1", "Huber"));
    }

    //result operators
    //(from c in _cooks select c).Count()
    //(from c in _cooks select c).Distinct()
    //(from c in _cooks select c).Take(5)
    //(from c in _cooks select c).Take(c.ID)
    //from k in _kitchen from c in k.Restaurant.Cooks.Take(k.RoomNumber) select c
    //(from c in _cooks select c).Single()
    //(from c in _cooks select c).First()

    //where conditions
    //from c in _cooks where c.Name = "Huber" select c.FirstName
    //from c in _cooks where c.Name = "Huber" && c.FirstName = "Sepp" select c;
    //(from c in _cooks where c.IsFullTimeCook select c)
    //(from c in _cooks where true select c)
    //(from c in _cooks where false select c)

    //binary expression
    //(from c in _cooks where c.Name == null select c)
    //(from c in _cooks where c.ID + c.ID select c)
    // see SqlGeneratingExpressionVisitor.VisitBinaryExpressions for further tests
    //(from c in _cooks where c.IsFullTimeCook == true select c)
    //(from c in _cooks where c.IsFullTimeCook == false select c)

    //unary expressions (unary plus, unary negate, unary not)
    //(from c in _cooks where (-c.ID) == -1 select c)
    //(from c in _cooks where !c.IsStarredCook == true select c)
    //(from c in _cooks where (+c.ID) == -1 select c)

    //method calls (review method)
    //SqlStatementTextGenerator.GenerateSqlGeneratorRegistry
  }
}