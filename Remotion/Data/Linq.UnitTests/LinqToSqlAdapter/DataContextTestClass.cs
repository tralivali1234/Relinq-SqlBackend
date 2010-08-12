﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace Remotion.Data.Linq.UnitTests.LinqToSqlAdapter
{
  [Database (Name = "DATACONTEXT")]
  internal class DataContextTestClass : DataContext
  {
    private static readonly MappingSource mappingSource = new AttributeMappingSource();

    // TODO: Remove commented code
    // TODO: Add inheritance hierarchy Contact/Customer-Supplier with discriminator column

    #region constructors

    static DataContextTestClass ()
    {
    }

    public DataContextTestClass (string connection)
        : base (connection, mappingSource)
    {
    }

    #endregion

    [Table (Name = "dbo.Customers")]
    public class Customer //: INotifyPropertyChanging, INotifyPropertyChanged
    {
      private string _CustomerID;
      private string _CompanyName;
      private readonly EntitySet<Order> _Orders = new EntitySet<Order>();

      [Column (Storage = "_CustomerID", DbType = "NChar(5) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
      public string CustomerID
      {
        get { return _CustomerID; }
        set
        {
          if ((_CustomerID != value))
          {
            //this.OnCustomerIDChanging (value);
            //this.SendPropertyChanging ();
            _CustomerID = value;
            //this.SendPropertyChanged ("CustomerID");
            //this.OnCustomerIDChanged ();
          }
        }
      }

      [Column (Storage = "_CompanyName", DbType = "NVarChar(40) NOT NULL", CanBeNull = false)]
      public string CompanyName
      {
        get { return _CompanyName; }
        set
        {
          if ((_CompanyName != value))
          {
            //this.OnCompanyNameChanging (value);
            //this.SendPropertyChanging ();
            _CompanyName = value;
            //this.SendPropertyChanged ("CompanyName");
            //this.OnCompanyNameChanged ();
          }
        }
      }

      [Association (Name = "Customer_Order", Storage = "_Orders", OtherKey = "CustomerID")]
      public EntitySet<Order> Orders
      {
        get { return _Orders; }
        set { _Orders.Assign (value); }
      }
    }

    [Table (Name = "dbo.Region")]
    public class Region //: INotifyPropertyChanging, INotifyPropertyChanged
    {
      private int _RegionID;

      private string _RegionDescription;

      private readonly EntitySet<Territory> _Territories = new EntitySet<Territory>();


      [Column (Storage = "_RegionID", DbType = "Int NOT NULL", IsPrimaryKey = true)]
      public int RegionID
      {
        get { return _RegionID; }
        set
        {
          if ((_RegionID != value))
          {
            //this.OnRegionIDChanging (value);
            //this.SendPropertyChanging ();
            _RegionID = value;
            //this.SendPropertyChanged ("RegionID");
            //this.OnRegionIDChanged ();
          }
        }
      }

      [Column (Storage = "_RegionDescription", DbType = "NChar(50) NOT NULL", CanBeNull = false)]
      public string RegionDescription
      {
        get { return _RegionDescription; }
        set
        {
          if ((_RegionDescription != value))
          {
            //this.OnRegionDescriptionChanging (value);
            //this.SendPropertyChanging ();
            _RegionDescription = value;
            //this.SendPropertyChanged ("RegionDescription");
            //this.OnRegionDescriptionChanged ();
          }
        }
      }

      [Association (Name = "Region_Territory", Storage = "_Territories", OtherKey = "RegionID")]
      public EntitySet<Territory> Territories
      {
        get { return _Territories; }
        set { _Territories.Assign (value); }
      }
    }

    [Table (Name = "dbo.Territories")]
    public class Territory //: INotifyPropertyChanging, INotifyPropertyChanged
    {
    }

    [Table (Name = "dbo.Orders")]
    public class Order // : INotifyPropertyChanging, INotifyPropertyChanged
    {
      private int _OrderID;
      private string _CustomerID;

      private EntityRef<Customer> _Customer;

      [Column (Storage = "_OrderID", AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
      public int OrderID
      {
        get { return _OrderID; }
        set
        {
          if ((_OrderID != value))
          {
            //this.OnOrderIDChanging (value);
            //this.SendPropertyChanging ();
            _OrderID = value;
            //this.SendPropertyChanged ("OrderID");
            //this.OnOrderIDChanged ();
          }
        }
      }

      [Column (Storage = "_CustomerID", DbType = "NChar(5)")]
      public string CustomerID
      {
        get { return _CustomerID; }
        set
        {
          if ((_CustomerID != value))
          {
            //if (this._Customer.HasLoadedOrAssignedValue)
            //{
            //throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException ();
            //}
            //this.OnCustomerIDChanging (value);
            //this.SendPropertyChanging ();
            _CustomerID = value;
            //this.SendPropertyChanged ("CustomerID");
            //this.OnCustomerIDChanged ();
          }
        }
      }

      [Association (Name = "Customer_Order", Storage = "_Customer", ThisKey = "CustomerID", IsForeignKey = true)]
      public Customer Customer
      {
        get { return _Customer.Entity; }
        set
        {
          Customer previousValue = _Customer.Entity;
          if (((previousValue != value)
               || (_Customer.HasLoadedOrAssignedValue == false)))
          {
            //this.SendPropertyChanging ();
            if ((previousValue != null))
            {
              _Customer.Entity = null;
              previousValue.Orders.Remove (this);
            }
            _Customer.Entity = value;
            if ((value != null))
            {
              value.Orders.Add (this);
              _CustomerID = value.CustomerID;
            }
            else
              _CustomerID = default (string);
            //this.SendPropertyChanged ("Customer");
          }
        }
      }
    }
  }

  //DataContextTestClass
}