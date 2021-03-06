﻿using System;

namespace Vexe.Runtime.Types
{
	/// <summary>
	/// Allows you to categories your members according to many rules that you could customize
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class DefineCategoryAttribute : Attribute
	{
		/// <summary>
		/// Allows you to include members by their MemberType i.e. Field, Property, Method or All
		/// You can 'or' (add) multiple member types together ex MemberType.Field | MemberType.Property
		/// </summary>
		public MemberType MemberType { get; set; }

		/// <summary>
		/// Allows you to include members whose name matches this regex pattern
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// Allows you to include members whose data type matches this type (ex methods that return int etc)
		/// </summary>
		public Type DataType { get; set; }

		/// <summary>
		/// The full path of this defintion split by '/'
		/// </summary>
		public string FullPath { get; private set; }

		/// <summary>
		/// Allows you to explictly include members by name
		/// </summary>
		public string[] ExplicitMembers { get; private set; }

		/// <summary>
		/// The descending sorting order of the defined category.
		/// </summary>
		public float DisplayOrder { get; set; }

		/// <summary>
		/// If true, the members for this category will not appear in other categories
		/// NOTE: Categories are sorted when they're processed such that exclusive ones come first
		/// Default: true
		/// </summary>
		public bool Exclusive { get; set; }

		/// <summary>
		/// Whether to perform a union or intersection on the definition rules
		/// Default: SetOp.Intersect
		/// </summary>
		public SetOp Grouping { get; set; }

		/// <summary>
		/// If true, the category will always be expanded
		/// </summary>
		public bool ForceExpand { get; set; }

		/// <summary>
		/// Category header shown?
		/// </summary>
		public bool AlwaysHideHeader { get; set; }

		public DefineCategoryAttribute(string fullPath, float displayOrder, params string[] explicitMembers)
		{
			FullPath = fullPath;
			DisplayOrder = displayOrder;
			ExplicitMembers = explicitMembers;
			Grouping = SetOp.Intersection;
			Exclusive = true;
		}

		public DefineCategoryAttribute(string name, params string[] explicitMembers)
			: this(name, -1, explicitMembers)
		{
		}
	}

	[Flags]
	public enum MemberType { None = 0, Field = 4, Method = 8, Property = 16, All = 28 }

	[Flags]
	public enum MembersDisplay
	{
	   MemberSplitter = 1 << 0, // split line between members
	   CategorySplitter = 1 << 1, // split line between categories
	   BoxedMembersArea = 1 << 2,	// a gui box wrapping categories
	   Headers = 1 << 3,	 // show category header?
	   BoxedHeaders = 1 << 4, // show headers in a gui box
	};

	public enum SetOp { Intersection, Union }
}
