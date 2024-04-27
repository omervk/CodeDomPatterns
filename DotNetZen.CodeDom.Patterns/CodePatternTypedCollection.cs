using System;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of a custom typed collection.
	/// </summary>
	/// <remarks>
	/// Please note: A bug in the .net frameworks up to (and not including) 2.0 does not allow for creation of parameter arrays,
	/// which means that the pattern is incomplete when used with these versions.
	/// </remarks>
	/// <example>Output example:<code>
	/// /// &lt;summary&gt;
	///	/// Represents a collection of &lt;see cref="Int32" /&gt; elements.
	///	/// &lt;/summary&gt;
	///	[System.SerializableAttribute()]
	///	public class Int32Collection : System.Collections.CollectionBase
	///	{
	///		/// &lt;summary&gt;
	///		/// See &lt;see cref="IsInLoad" /&gt; for information about this field.
	///		/// &lt;/summary&gt;
	///		private int m_IsInLoad;
	///	    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="Int32Collection" /&gt; class.
	///		/// &lt;/summary&gt;
	///		public Int32Collection()
	///		{
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="Int32Collection" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="values"&gt;A list of objects of type &lt;see cref="Int32" /&gt; to initialize the collection with.&lt;/param&gt;
	///		public Int32Collection(params int[] values)
	///		{
	///			if ((values == null))
	///			{
	///				throw new System.ArgumentNullException("values");
	///			}
	///			this.InnerList.Capacity = values.Length;
	///			this.BeginLoad();
	///			try
	///			{
	///				this.AddRange(values);
	///			}
	///			finally
	///			{
	///				this.EndLoad();
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="Int32Collection" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="collection"&gt;An object of type &lt;see cref="Int32Collection" /&gt; containing objects to be copied into the new collection.&lt;/param&gt;
	///		public Int32Collection(Int32Collection collection)
	///		{
	///			if ((collection == null))
	///			{
	///				throw new System.ArgumentNullException("collection");
	///			}
	///			this.InnerList.Capacity = collection.Count;
	///			this.BeginLoad();
	///			try
	///			{
	///				this.AddRange(collection);
	///			}
	///			finally
	///			{
	///				this.EndLoad();
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Gets or sets the &lt;see cref="Int32" /&gt; at position &lt;paramref name="index" /&gt;.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;The &lt;see cref="Int32" /&gt; at position &lt;paramref name="index" /&gt;.&lt;/value&gt;
	///		/// &lt;param name="index"&gt;The position of the &lt;see cref="Int32" /&gt;.&lt;/param&gt;
	///		public int this[int index]
	///		{
	///			get
	///			{
	///				return ((int)(this.List[index]));
	///			}
	///			set
	///			{
	///				this.List[index] = value;
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Gets an object that can be used to synchronize access to the collection.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;An object that can be used to synchronize access to the collection.&lt;/value&gt;
	///		public object SyncRoot
	///		{
	///			get
	///			{
	///				return this.List.SyncRoot;
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs when the collection begins the process of removing all elements.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;To indicate that clearing the collection is not possible, throw an exception from an attached method.&lt;/remarks&gt;
	///		public event System.EventHandler Clearing;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs after the collection completes the process of removing all elements.
	///		/// &lt;/summary&gt;
	///		public event System.EventHandler Cleared;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs when the collection begins the process of inserting an element.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;To indicate that inserting this element is invalid, throw an exception from an attached method. To indicate that the element is simply invalid, use the &lt;see cref="Validating" /&gt; event.&lt;/remarks&gt;
	///		public event EventHandler Inserting;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs after the collection completes the process of inserting an element.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;To prevent the insertion from taking place, throw an exception from an attached method.&lt;/remarks&gt;
	///		public event EventHandler Inserted;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs when the collection begins the process of removing an element.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;To indicate that removing this element is invalid, throw an exception from an attached method. To indicate that the element is simply invalid, use the &lt;see cref="Validating" /&gt; event.&lt;/remarks&gt;
	///		public event EventHandler Removing;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs after the collection completes the process of removing an element.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;To prevent the removal from taking place, throw an exception from an attached method.&lt;/remarks&gt;
	///		public event EventHandler Removed;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs when the collection begins the process of setting an element at a certain position.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;To indicate that setting this element at this position is invalid, throw an exception from an attached method. To indicate that the element is simply invalid, use the &lt;see cref="Validating" /&gt; event.&lt;/remarks&gt;
	///		public event SetEventHandler Setting;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs after the collection completes the process of setting an element at a certain position.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;To prevent the settng action from taking place, throw an exception from an attached method.&lt;/remarks&gt;
	///		public event SetEventHandler Set;
	///	    
	///		/// &lt;summary&gt;
	///		/// Occurs when the collection asks for validation of an item that is to be added into it.
	///		/// &lt;/summary&gt;
	///		/// &lt;remarks&gt;If the item is invalid, throw an exception from an attached method.
	///		/// Checks that already take place are that the value is not a null reference (Nothing in Visual Basic) and that it is of/derives from &lt;see cref="Int32" /&gt;.&lt;/remarks&gt;
	///		public event ValidationEventHandler Validating;
	///	    
	///		/// &lt;summary&gt;
	///		/// Begins the Load process.
	///		/// &lt;/summary&gt;
	///		public virtual void BeginLoad()
	///		{
	///			this.m_IsInLoad = (this.m_IsInLoad + 1);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Ends the Load process.
	///		/// &lt;/summary&gt;
	///		public virtual void EndLoad()
	///		{
	///			if ((this.m_IsInLoad != 0))
	///			{
	///				this.m_IsInLoad = (this.m_IsInLoad - 1);
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Gets whether the Load process has begun.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;Whether the Load process has begun.&lt;/value&gt;
	///		protected bool IsInLoad()
	///		{
	///			return (this.m_IsInLoad != 0);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Inserts an element into the collection at the specified index.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="index"&gt;The zero-based index at which &lt;paramref name="value" /&gt; should be inserted.&lt;/param&gt;
	///		/// &lt;param name="value"&gt;The &lt;see cref="Int32" /&gt; to insert.&lt;/param&gt;
	///		/// &lt;exception cref="System.ArgumentOutOfRangeException"&gt;&lt;paramref name="index" /&gt; is less than zero or is greater than &lt;see cref="System.Collections.CollectionBase.Count" /&gt;.&lt;/exception&gt;
	///		public void Insert(int index, int value)
	///		{
	///			this.List.Insert(index, value);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Removes the first occurrence of a specific &lt;see cref="Int32" /&gt; from the collection.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="value"&gt;The &lt;see cref="Int32" /&gt; to remove from the collection.&lt;/param&gt;
	///		public void Remove(int value)
	///		{
	///			this.List.Remove(value);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Determines whether an element is in the collection.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="value"&gt;The &lt;see cref="Int32" /&gt; to locate in the collection.&lt;/param&gt;
	///		/// &lt;returns&gt;true if &lt;paramref name="value" /&gt; is found in the collection; otherwise, false.&lt;/returns&gt;
	///		public bool Contains(int value)
	///		{
	///			return this.List.Contains(value);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Searches for the specified &lt;see cref="Int32" /&gt; and returns the zero-based index of the first occurrence within the entire collection.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="value"&gt;The &lt;see cref="Int32" /&gt; to locate in the collection.&lt;/param&gt;
	///		/// &lt;returns&gt;The zero-based index of the first occurrence of &lt;paramref name="value" /&gt; within the entire collection, if found; otherwise, -1.&lt;/returns&gt;
	///		public int IndexOf(int value)
	///		{
	///			return this.List.IndexOf(value);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Adds an object to the end of the collection.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="value"&gt;The &lt;see cref="Int32" /&gt; to be added to the end of the collection.&lt;/param&gt;
	///		/// &lt;returns&gt;The collection index at which the &lt;paramref name="value" /&gt; has been added.&lt;/returns&gt;
	///		public int Add(int value)
	///		{
	///			return this.List.Add(value);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Adds the elements of an array to the end of the collection.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="values"&gt;The array whose elements should be added to the end of the collection.&lt;/param&gt;
	///		/// &lt;exception cref="System.ArgumentNullException"&gt;&lt;paramref name="values" /&gt; is a null reference (Nothing in Visual Basic).&lt;/exception&gt;
	///		public void AddRange(int[] values)
	///		{
	///			if ((values == null))
	///			{
	///				throw new System.ArgumentNullException("values");
	///			}
	///			System.Collections.IEnumerator enumerator0 = ((System.Collections.IEnumerable)(values)).GetEnumerator();
	///			try
	///			{
	///				for (
	///					; (enumerator0.MoveNext() == true); 
	///					)
	///				{
	///					int element0 = ((int)(enumerator0.Current));
	///					this.List.Add(element0);
	///				}
	///			}
	///			finally
	///			{
	///				if (((enumerator0 != null) 
	///					&amp;&amp; enumerator0.GetType().IsInstanceOfType(typeof(System.IDisposable))))
	///				{
	///					((System.IDisposable)(enumerator0)).Dispose();
	///				}
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Adds the elements of a collection to the end of the collection.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="collection"&gt;The collection whose elements should be added to the end of the collection.&lt;/param&gt;
	///		/// &lt;exception cref="System.ArgumentNullException"&gt;&lt;paramref name="collection" /&gt; is a null reference (Nothing in Visual Basic).&lt;/exception&gt;
	///		public void AddRange(Int32Collection collection)
	///		{
	///			if ((collection == null))
	///			{
	///				throw new System.ArgumentNullException("collection");
	///			}
	///			System.Collections.IEnumerator enumerator1 = ((System.Collections.IEnumerable)(collection.InnerList)).GetEnumerator();
	///			try
	///			{
	///				for (
	///					; (enumerator1.MoveNext() == true); 
	///					)
	///				{
	///					int element1 = ((int)(enumerator1.Current));
	///					this.List.Add(element1);
	///				}
	///			}
	///			finally
	///			{
	///				if (((enumerator1 != null) 
	///					&amp;&amp; enumerator1.GetType().IsInstanceOfType(typeof(System.IDisposable))))
	///				{
	///					((System.IDisposable)(enumerator1)).Dispose();
	///				}
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Copies the entire collection to a compatible one-dimensional &lt;see cref="System.Array" /&gt;, starting at the specified &lt;paramref name="index" /&gt; of the target array.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="array"&gt;The one-dimensional &lt;see cref="System.Array" /&gt; that is the destination of the elements copied from collection. The &lt;see cref="System.Array" /&gt; must have zero-based indexing.&lt;/param&gt;
	///		/// &lt;param name="index"&gt;The zero-based index in &lt;paramref name="array" /&gt; at which copying begins.&lt;/param&gt;
	///		/// &lt;exception cref="System.ArgumentNullException"&gt;&lt;paramref name="array" /&gt; is a null reference (Nothing in Visual Basic).&lt;/exception&gt;
	///		/// &lt;exception cref="System.ArgumentOutOfRangeException"&gt;&lt;paramref name="index" /&gt; is less than zero.&lt;/exception&gt;
	///		/// &lt;exception cref="System.ArgumentException"&gt;&lt;paramref name="array" /&gt; is multidimensional.&lt;/exception&gt;
	///		/// &lt;exception cref="System.ArgumentException"&gt;&lt;paramref name="index" /&gt; is equal to or greater than the length of &lt;paramref name="array" /&gt;.&lt;/exception&gt;
	///		/// &lt;exception cref="System.ArgumentException"&gt;The number of elements in the source collection is greater than the available space from &lt;paramref name="index" /&gt; to the end of the destination &lt;paramref name="array" /&gt;.&lt;/exception&gt;
	///		/// &lt;remarks&gt;The specified array's elements must be of type &lt;see cref="Int32" /&gt; or any of its derivatives.&lt;/remarks&gt;
	///		public void CopyTo(System.Array array, int index)
	///		{
	///			this.List.CopyTo(array, index);
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Copies the elements of the collection to a new &lt;see cref="Int32" /&gt; array.
	///		/// &lt;/summary&gt;
	///		/// &lt;returns&gt;An array of &lt;see cref="Int32" /&gt; containing copies of the elements of the collection.&lt;/returns&gt;
	///		public int[] ToArray()
	///		{
	///			return ((int[])(this.InnerList.ToArray(typeof(Int32))));
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Clearing" /&gt; event.
	///		/// &lt;/summary&gt;
	///		protected override void OnClear()
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Clearing != null)))
	///			{
	///				this.Clearing(this, System.EventArgs.Empty);
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Cleared" /&gt; event.
	///		/// &lt;/summary&gt;
	///		protected override void OnClearComplete()
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Cleared != null)))
	///			{
	///				this.Cleared(this, System.EventArgs.Empty);
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Inserting" /&gt; event.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="index"&gt;The zero-based index at which to insert &lt;paramref name="value" /&gt;.&lt;/param&gt;
	///		/// &lt;param name="value"&gt;The new value of the element at &lt;paramref name="index" /&gt;.&lt;/param&gt;
	///		protected override void OnInsert(int index, object value)
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Inserting != null)))
	///			{
	///				this.Inserting(this, new EventArgs(index, ((int)(value))));
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Inserted" /&gt; event.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="index"&gt;The zero-based index at which to insert &lt;paramref name="value" /&gt;.&lt;/param&gt;
	///		/// &lt;param name="value"&gt;The new value of the element at &lt;paramref name="index" /&gt;.&lt;/param&gt;
	///		protected override void OnInsertComplete(int index, object value)
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Inserted != null)))
	///			{
	///				this.Inserted(this, new EventArgs(index, ((int)(value))));
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Removing" /&gt; event.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="index"&gt;The zero-based index at which &lt;paramref name="value" /&gt; can be found.&lt;/param&gt;
	///		/// &lt;param name="value"&gt;The value of the element to remove from &lt;paramref name="index" /&gt;.&lt;/param&gt;
	///		protected override void OnRemove(int index, object value)
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Removing != null)))
	///			{
	///				this.Removing(this, new EventArgs(index, ((int)(value))));
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Removed" /&gt; event.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="index"&gt;The zero-based index at which &lt;paramref name="value" /&gt; can be found.&lt;/param&gt;
	///		/// &lt;param name="value"&gt;The value of the element to remove from &lt;paramref name="index" /&gt;.&lt;/param&gt;
	///		protected override void OnRemoveComplete(int index, object value)
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Removed != null)))
	///			{
	///				this.Removed(this, new EventArgs(index, ((int)(value))));
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Setting" /&gt; event.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="index"&gt;The zero-based index at which &lt;paramref name="oldValue" /&gt; can be found.&lt;/param&gt;
	///		/// &lt;param name="oldValue"&gt;The value to replace with &lt;paramref name="newValue" /&gt;.&lt;/param&gt;
	///		/// &lt;param name="newValue"&gt;The new value of the element at &lt;paramref name="index" /&gt;.&lt;/param&gt;
	///		protected override void OnSet(int index, object oldValue, object newValue)
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Setting != null)))
	///			{
	///				this.Setting(this, new SetEventArgs(index, ((int)(oldValue)), ((int)(newValue))));
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Set" /&gt; event.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="index"&gt;The zero-based index at which &lt;paramref name="oldValue" /&gt; can be found.&lt;/param&gt;
	///		/// &lt;param name="oldValue"&gt;The value to replace with &lt;paramref name="newValue" /&gt;.&lt;/param&gt;
	///		/// &lt;param name="newValue"&gt;The new value of the element at &lt;paramref name="index" /&gt;.&lt;/param&gt;
	///		protected override void OnSetComplete(int index, object oldValue, object newValue)
	///		{
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Set != null)))
	///			{
	///				this.Set(this, new SetEventArgs(index, ((int)(oldValue)), ((int)(newValue))));
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Raises the &lt;see cref="Validating" /&gt; event.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="value"&gt;The object to validate.&lt;/param&gt;
	///		protected override void OnValidate(object value)
	///		{
	///			base.OnValidate(value);
	///			if ((value.GetType().IsInstanceOfType(typeof(int)) == false))
	///			{
	///				throw new System.ArgumentException(string.Format("The argument value must be of type {0}.", typeof(int).FullName), "value");
	///			}
	///			if (((this.IsInLoad() == false) 
	///				&amp;&amp; (this.Validating != null)))
	///			{
	///				this.Validating(this, new ValidationEventArgs(((int)(value))));
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Represents a method that takes a &lt;see cref="System.Object" /&gt; and &lt;see cref="EventArgs" /&gt;.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="sender"&gt;The event's originating object.&lt;/param&gt;
	///		/// &lt;param name="e"&gt;The event's arguments.&lt;/param&gt;
	///		public delegate void EventHandler(object sender, EventArgs e);
	///	    
	///		/// &lt;summary&gt;
	///		/// Contains the arguments for events based on the &lt;see cref="EventHandler" /&gt; delegate.
	///		/// &lt;/summary&gt;
	///		public class EventArgs : System.EventArgs
	///		{
	///			/// &lt;summary&gt;
	///			/// Value for the property &lt;see cref="Index" /&gt;.
	///			/// &lt;/summary&gt;
	///			private int m_Index;
	///	        
	///			/// &lt;summary&gt;
	///			/// Value for the property &lt;see cref="Value" /&gt;.
	///			/// &lt;/summary&gt;
	///			private int m_Value;
	///	        
	///			/// &lt;summary&gt;
	///			/// Initializes a new instance of the &lt;see cref="EventArgs" /&gt; class.
	///			/// &lt;/summary&gt;
	///			/// &lt;param name="Index"&gt;The index of the value.&lt;/param&gt;
	///			/// &lt;param name="Value"&gt;The value raised by the event.&lt;/param&gt;
	///			public EventArgs(int Index, int Value)
	///			{
	///				this.m_Index = Index;
	///				this.m_Value = Value;
	///			}
	///	        
	///			/// &lt;summary&gt;
	///			/// Gets the index of the value.
	///			/// &lt;/summary&gt;
	///			/// &lt;value&gt;The index of the value.&lt;/value&gt;
	///			public virtual int Index
	///			{
	///				get
	///				{
	///					return this.m_Index;
	///				}
	///			}
	///	        
	///			/// &lt;summary&gt;
	///			/// Gets the value raised by the event.
	///			/// &lt;/summary&gt;
	///			/// &lt;value&gt;The value raised by the event.&lt;/value&gt;
	///			public virtual int Value
	///			{
	///				get
	///				{
	///					return this.m_Value;
	///				}
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Represents a method that takes a &lt;see cref="System.Object" /&gt; and &lt;see cref="SetEventArgs" /&gt;.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="sender"&gt;The event's originating object.&lt;/param&gt;
	///		/// &lt;param name="e"&gt;The event's arguments.&lt;/param&gt;
	///		public delegate void SetEventHandler(object sender, SetEventArgs e);
	///	    
	///		/// &lt;summary&gt;
	///		/// Contains the arguments for events based on the &lt;see cref="SetEventHandler" /&gt; delegate.
	///		/// &lt;/summary&gt;
	///		public class SetEventArgs : System.EventArgs
	///		{
	///			/// &lt;summary&gt;
	///			/// Value for the property &lt;see cref="Index" /&gt;.
	///			/// &lt;/summary&gt;
	///			private int m_Index;
	///	        
	///			/// &lt;summary&gt;
	///			/// Value for the property &lt;see cref="OldValue" /&gt;.
	///			/// &lt;/summary&gt;
	///			private int m_OldValue;
	///	        
	///			/// &lt;summary&gt;
	///			/// Value for the property &lt;see cref="NewValue" /&gt;.
	///			/// &lt;/summary&gt;
	///			private int m_NewValue;
	///	        
	///			/// &lt;summary&gt;
	///			/// Initializes a new instance of the &lt;see cref="SetEventArgs" /&gt; class.
	///			/// &lt;/summary&gt;
	///			/// &lt;param name="Index"&gt;The zero-based index at which oldValue can be found.&lt;/param&gt;
	///			/// &lt;param name="OldValue"&gt;The value to replace with newValue.&lt;/param&gt;
	///			/// &lt;param name="NewValue"&gt;The new value of the element at index.&lt;/param&gt;
	///			public SetEventArgs(int Index, int OldValue, int NewValue)
	///			{
	///				this.m_Index = Index;
	///				this.m_OldValue = OldValue;
	///				this.m_NewValue = NewValue;
	///			}
	///	        
	///			/// &lt;summary&gt;
	///			/// Gets the zero-based index at which oldValue can be found.
	///			/// &lt;/summary&gt;
	///			/// &lt;value&gt;The zero-based index at which oldValue can be found.&lt;/value&gt;
	///			public virtual int Index
	///			{
	///				get
	///				{
	///					return this.m_Index;
	///				}
	///			}
	///	        
	///			/// &lt;summary&gt;
	///			/// Gets the value to replace with newValue.
	///			/// &lt;/summary&gt;
	///			/// &lt;value&gt;The value to replace with newValue.&lt;/value&gt;
	///			public virtual int OldValue
	///			{
	///				get
	///				{
	///					return this.m_OldValue;
	///				}
	///			}
	///	        
	///			/// &lt;summary&gt;
	///			/// Gets the new value of the element at index.
	///			/// &lt;/summary&gt;
	///			/// &lt;value&gt;The new value of the element at index.&lt;/value&gt;
	///			public virtual int NewValue
	///			{
	///				get
	///				{
	///					return this.m_NewValue;
	///				}
	///			}
	///		}
	///	    
	///		/// &lt;summary&gt;
	///		/// Represents a method that takes a &lt;see cref="System.Object" /&gt; and &lt;see cref="ValidationEventArgs" /&gt;.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="sender"&gt;The event's originating object.&lt;/param&gt;
	///		/// &lt;param name="e"&gt;The event's arguments.&lt;/param&gt;
	///		public delegate void ValidationEventHandler(object sender, ValidationEventArgs e);
	///	    
	///		/// &lt;summary&gt;
	///		/// Contains the arguments for events based on the &lt;see cref="ValidationEventHandler" /&gt; delegate.
	///		/// &lt;/summary&gt;
	///		public class ValidationEventArgs : System.EventArgs
	///		{
	///			/// &lt;summary&gt;
	///			/// Value for the property &lt;see cref="Value" /&gt;.
	///			/// &lt;/summary&gt;
	///			private int m_Value;
	///	        
	///			/// &lt;summary&gt;
	///			/// Initializes a new instance of the &lt;see cref="ValidationEventArgs" /&gt; class.
	///			/// &lt;/summary&gt;
	///			/// &lt;param name="Value"&gt;The value to be validated.&lt;/param&gt;
	///			public ValidationEventArgs(int Value)
	///			{
	///				this.m_Value = Value;
	///			}
	///	        
	///			/// &lt;summary&gt;
	///			/// Gets the value to be validated.
	///			/// &lt;/summary&gt;
	///			/// &lt;value&gt;The value to be validated.&lt;/value&gt;
	///			public virtual int Value
	///			{
	///				get
	///				{
	///					return this.m_Value;
	///				}
	///			}
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternTypedCollection : CodeTypeDeclaration
	{
		#region Constant Names
		private static readonly string BaseListPropertyName = typeof(CollectionBase).GetProperty("List", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseInnerListPropertyName = typeof(CollectionBase).GetProperty("InnerList", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnClearMethodName = typeof(CollectionBase).GetMethod("OnClear", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnClearCompleteMethodName = typeof(CollectionBase).GetMethod("OnClearComplete", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnInsertMethodName = typeof(CollectionBase).GetMethod("OnInsert", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnInsertCompleteMethodName = typeof(CollectionBase).GetMethod("OnInsertComplete", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnRemoveMethodName = typeof(CollectionBase).GetMethod("OnRemove", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnRemoveCompleteMethodName = typeof(CollectionBase).GetMethod("OnRemoveComplete", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnSetMethodName = typeof(CollectionBase).GetMethod("OnSet", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnSetCompleteMethodName = typeof(CollectionBase).GetMethod("OnSetComplete", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string BaseOnValidateMethodName = typeof(CollectionBase).GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.NonPublic).Name;
		private static readonly string ArrayListCapacityPropertyName = typeof(ArrayList).GetProperty("Capacity").Name;
		private static readonly string ArrayListToArrayPropertyName = typeof(ArrayList).GetMethod("ToArray", new Type[] { typeof(Type) }).Name;
		private static readonly string ArrayLengthPropertyName = typeof(Array).GetProperty("Length").Name;
		private static readonly string ICollectionCountPropertyName = typeof(ICollection).GetProperty("Count").Name;
		private static readonly string ICollectionSyncRootPropertyName = typeof(ICollection).GetProperty("SyncRoot").Name;
		private static readonly string ICollectionCopyToMethodName = typeof(ICollection).GetMethod("CopyTo").Name;
		private static readonly string IListAddMethodName = typeof(IList).GetMethod("Add").Name;
		private static readonly string IListInsertMethodName = typeof(IList).GetMethod("Insert").Name;
		private static readonly string IListRemoveMethodName = typeof(IList).GetMethod("Remove").Name;
		private static readonly string IListContainsMethodName = typeof(IList).GetMethod("Contains").Name;
		private static readonly string IListIndexOfMethodName = typeof(IList).GetMethod("IndexOf").Name;
		private static readonly string IListItemPropertyName = typeof(IList).GetProperty("Item").Name;
		private static readonly string EventArgsEmptyFieldName = typeof(EventArgs).GetField("Empty").Name;
		private const string CollectionSuffix = "Collection";
		private const string BeginEndLoadProcessName = "Load";
		private const string ArrayVariableName = "values";
		private const string SimpleArrayVariableName = "array";
		private const string CollectionVariableName = "collection";
		private const string AddRangeMethodName = "AddRange";
		private const string IndexVariableName = "index";
		private const string ValueVariableName = "value";
		private const string ToArrayMethodName = "ToArray";
		private const string ClearingEventName = "Clearing";
		private const string ClearedEventName = "Cleared";
		private const string InsertingEventName = "Inserting";
		private const string InsertedEventName = "Inserted";
		private const string RemovingEventName = "Removing";
		private const string RemovedEventName = "Removed";
		private const string SettingEventName = "Setting";
		private const string SetEventName = "Set";
		private const string ValidatingEventName = "Validating";
		private const string EventArgsIndexPropertyName = "Index";
		private const string EventArgsValuePropertyName = "Value";
		private const string EventArgsNewValuePropertyName = "NewValue";
		private const string EventArgsOldValuePropertyName = "OldValue";
		private const string SetEventHandlerPrefix = "Set";
		private const string ValidationEventHandlerPrefix = "Validation";
		private const string OldValueVariableName = "oldValue";
		private const string NewValueVariableName = "newValue";
		#endregion

		#region Private Members
		// Standard members
		private CodeConstructor parameterlessConstructor;
		private CodeConstructor arrayConstructor;
		private CodeConstructor collectionConstructor;
		private CodeMemberProperty indexer;
		private CodeMemberMethod insert;
		private CodeMemberMethod remove;
		private CodeMemberMethod contains;
		private CodeMemberMethod indexOf;
		private CodeMemberMethod add;
		private CodeMemberMethod addRangeArray;
		private CodeMemberMethod addRangeCollection;
		private CodeMemberMethod copyTo;
		private CodeMemberProperty syncRoot;
		private CodeMemberMethod toArray;
		
		// Events
		private CodePatternBeginEndProcess beginEndLoad;
		private CodePatternDelegate eventHandler;
		private CodePatternDelegate setEventHandler;
		private CodePatternDelegate validationEventHandler;
		private CodeMemberEvent clearing;
		private CodeMemberEvent cleared;
		private CodeMemberEvent inserting;
		private CodeMemberEvent inserted;
		private CodeMemberEvent removing;
		private CodeMemberEvent removed;
		private CodeMemberEvent setting;
		private CodeMemberEvent set;
		private CodeMemberEvent validating;
		private CodeMemberMethod onClearing;
		private CodeMemberMethod onCleared;
		private CodeMemberMethod onInserting;
		private CodeMemberMethod onInserted;
		private CodeMemberMethod onRemoving;
		private CodeMemberMethod onRemoved;
		private CodeMemberMethod onSetting;
		private CodeMemberMethod onSet;
		private CodeMemberMethod onValidating;

		private string typeName;
		#endregion

		private CodePatternTypedCollection(CodeTypeReference type, string typeName, CollectionEvents events) : base(typeName + CollectionSuffix)
		{
			CodeThisReferenceExpression @this = new CodeThisReferenceExpression();
			CodeTypeReference arrayType = new CodeTypeReference(type, 1);
			CodeTypeReference collectionType = CodeTypeReferenceStore.Get(this.Name);
			CodePropertyReferenceExpression thisList = new CodePropertyReferenceExpression(@this, BaseListPropertyName);

			this.typeName = typeName;

			this.BaseTypes.Add(typeof(CollectionBase));
			this.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(SerializableAttribute).FullName));

			#region Constructors, Properties and Methods
			if (events != CollectionEvents.None)
			{
				// Begin/EndInit
				this.beginEndLoad = new CodePatternBeginEndProcess(BeginEndLoadProcessName);
				this.Members.AddRange(this.beginEndLoad);
			}

			// public MyTypeCollection()
			this.parameterlessConstructor = new CodeConstructor();
			this.parameterlessConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.parameterlessConstructor.Attributes |= MemberAttributes.Public;
			this.Members.Add(this.parameterlessConstructor);

			// public MyTypeCollection(params MyType[] values)
			this.arrayConstructor = new CodeConstructor();
			this.arrayConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.arrayConstructor.Attributes |= MemberAttributes.Public;
			this.arrayConstructor.Parameters.Add(new CodeParameterDeclarationExpression(arrayType, ArrayVariableName));
			this.arrayConstructor.Parameters[0].CustomAttributes.Add(new CodeAttributeDeclaration(typeof(ParamArrayAttribute).FullName));
			this.arrayConstructor.Statements.Add(new CodePatternArgumentAssertNotNullStatement(ArrayVariableName));
			this.arrayConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(@this, BaseInnerListPropertyName), ArrayListCapacityPropertyName), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(ArrayVariableName), ArrayLengthPropertyName)));
			
			if (this.beginEndLoad != null)
			{
				this.arrayConstructor.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, this.beginEndLoad.Begin.Name)));
				this.arrayConstructor.Statements.Add(new CodeTryCatchFinallyStatement(
					new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, AddRangeMethodName, new CodeVariableReferenceExpression(ArrayVariableName) )) },
					new CodeCatchClause[0],
					new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, this.beginEndLoad.End.Name)) }));
			}
			else
			{
				this.arrayConstructor.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, AddRangeMethodName, new CodeVariableReferenceExpression(ArrayVariableName) )));
			}

			this.Members.Add(this.arrayConstructor);

			// public MyTypeCollection(MyTypeCollection collection)
			this.collectionConstructor = new CodeConstructor();
			this.collectionConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.collectionConstructor.Attributes |= MemberAttributes.Public;
			this.collectionConstructor.Parameters.Add(new CodeParameterDeclarationExpression(collectionType, CollectionVariableName));
			this.collectionConstructor.Statements.Add(new CodePatternArgumentAssertNotNullStatement(CollectionVariableName));
			this.collectionConstructor.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(@this, BaseInnerListPropertyName), ArrayListCapacityPropertyName), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(CollectionVariableName), ICollectionCountPropertyName)));

			if (this.beginEndLoad != null)
			{
				this.collectionConstructor.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, this.beginEndLoad.Begin.Name)));
				this.collectionConstructor.Statements.Add(new CodeTryCatchFinallyStatement(
					new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, AddRangeMethodName, new CodeVariableReferenceExpression(CollectionVariableName) )) },
					new CodeCatchClause[0],
					new CodeStatement[] { new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, this.beginEndLoad.End.Name)) }));
			}
			else
			{
				this.collectionConstructor.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(@this, AddRangeMethodName, new CodeVariableReferenceExpression(CollectionVariableName) )));
			}

			this.Members.Add(this.collectionConstructor);

			// public MyType this[int index] { get; set; }
			this.indexer = new CodeMemberProperty();
			this.indexer.Name = IListItemPropertyName;
			this.indexer.Attributes &= ~MemberAttributes.AccessMask;
			this.indexer.Attributes |= MemberAttributes.Public;
			this.indexer.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
			this.indexer.Type = type;
			this.indexer.HasGet = true;
			this.indexer.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(type, new CodeIndexerExpression(thisList, new CodeVariableReferenceExpression(IndexVariableName)))));
			this.indexer.HasSet = true;
			this.indexer.SetStatements.Add(new CodeAssignStatement(new CodeIndexerExpression(thisList, new CodeVariableReferenceExpression(IndexVariableName)), new CodePropertySetValueReferenceExpression()));
			this.Members.Add(this.indexer);

			// public void Insert(int index, [MyType] value)
			this.insert = new CodeMemberMethod();
			this.insert.Name = IListInsertMethodName;
			this.insert.Attributes &= ~MemberAttributes.AccessMask;
			this.insert.Attributes |= MemberAttributes.Public;
			this.insert.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
			this.insert.Parameters.Add(new CodeParameterDeclarationExpression(type, ValueVariableName));
			this.insert.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(thisList, IListInsertMethodName, new CodeVariableReferenceExpression(IndexVariableName), new CodeVariableReferenceExpression(ValueVariableName))));
			this.Members.Add(this.insert);

			// public void Remove(MyType value)
			this.remove = new CodeMemberMethod();
			this.remove.Name = IListRemoveMethodName;
			this.remove.Attributes &= ~MemberAttributes.AccessMask;
			this.remove.Attributes |= MemberAttributes.Public;
			this.remove.Parameters.Add(new CodeParameterDeclarationExpression(type, ValueVariableName));
			this.remove.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(thisList, IListRemoveMethodName, new CodeVariableReferenceExpression(ValueVariableName))));
			this.Members.Add(this.remove);

			// public bool Contains(MyType value)
			this.contains = new CodeMemberMethod();
			this.contains.Name = IListContainsMethodName;
			this.contains.Attributes &= ~MemberAttributes.AccessMask;
			this.contains.Attributes |= MemberAttributes.Public;
			this.contains.ReturnType = CodeTypeReferenceStore.Get(typeof(bool));
			this.contains.Parameters.Add(new CodeParameterDeclarationExpression(type, ValueVariableName));
			this.contains.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(thisList, IListContainsMethodName, new CodeVariableReferenceExpression(ValueVariableName))));
			this.Members.Add(this.contains);

			// public int IndexOf(MyType value)
			this.indexOf = new CodeMemberMethod();
			this.indexOf.Name = IListIndexOfMethodName;
			this.indexOf.Attributes &= ~MemberAttributes.AccessMask;
			this.indexOf.Attributes |= MemberAttributes.Public;
			this.indexOf.ReturnType = CodeTypeReferenceStore.Get(typeof(int));
			this.indexOf.Parameters.Add(new CodeParameterDeclarationExpression(type, ValueVariableName));
			this.indexOf.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(thisList, IListIndexOfMethodName, new CodeVariableReferenceExpression(ValueVariableName))));
			this.Members.Add(this.indexOf);

			// public int Add(MyType value)
			this.add = new CodeMemberMethod();
			this.add.Name = IListAddMethodName;
			this.add.Attributes &= ~MemberAttributes.AccessMask;
			this.add.Attributes |= MemberAttributes.Public;
			this.add.ReturnType = CodeTypeReferenceStore.Get(typeof(int));
			this.add.Parameters.Add(new CodeParameterDeclarationExpression(type, ValueVariableName));
			this.add.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(thisList, IListAddMethodName, new CodeVariableReferenceExpression(ValueVariableName))));
			this.Members.Add(this.add);

			// public void AddRange(MyType[] values)
			CodePatternForEach forEach = new CodePatternForEach(type, new CodeVariableReferenceExpression(ArrayVariableName));
			forEach.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(thisList, IListAddMethodName, new CodeVariableReferenceExpression(forEach.CurrentElementName))));
			
			this.addRangeArray = new CodeMemberMethod();
			this.addRangeArray.Name = AddRangeMethodName;
			this.addRangeArray.Attributes &= ~MemberAttributes.AccessMask;
			this.addRangeArray.Attributes |= MemberAttributes.Public;
			this.addRangeArray.Parameters.Add(new CodeParameterDeclarationExpression(arrayType, ArrayVariableName));
			this.addRangeArray.Statements.Add(new CodePatternArgumentAssertNotNullStatement(ArrayVariableName));
			this.addRangeArray.Statements.AddRange(forEach);
			this.Members.Add(this.addRangeArray);

			// public void AddRange(MyTypeCollection collection)
			forEach = new CodePatternForEach(type, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(CollectionVariableName), BaseInnerListPropertyName));
			forEach.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(thisList, IListAddMethodName, new CodeVariableReferenceExpression(forEach.CurrentElementName))));

			this.addRangeCollection = new CodeMemberMethod();
			this.addRangeCollection.Name = AddRangeMethodName;
			this.addRangeCollection.Attributes &= ~MemberAttributes.AccessMask;
			this.addRangeCollection.Attributes |= MemberAttributes.Public;
			this.addRangeCollection.Parameters.Add(new CodeParameterDeclarationExpression(this.Name, CollectionVariableName));
			this.addRangeCollection.Statements.Add(new CodePatternArgumentAssertNotNullStatement(CollectionVariableName));
			this.addRangeCollection.Statements.AddRange(forEach);
			this.Members.Add(this.addRangeCollection);

			// public void CopyTo(Array array, int index)
			this.copyTo = new CodeMemberMethod();
			this.copyTo.Name = ICollectionCopyToMethodName;
			this.copyTo.Attributes &= ~MemberAttributes.AccessMask;
			this.copyTo.Attributes |= MemberAttributes.Public;
			this.copyTo.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Array), SimpleArrayVariableName));
			this.copyTo.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
			this.copyTo.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(thisList, ICollectionCopyToMethodName, new CodeVariableReferenceExpression(SimpleArrayVariableName), new CodeVariableReferenceExpression(IndexVariableName))));
			this.Members.Add(this.copyTo);

			// public object SyncRoot
			this.syncRoot = new CodeMemberProperty();
			this.syncRoot.Name = ICollectionSyncRootPropertyName;
			this.syncRoot.Attributes &= ~MemberAttributes.AccessMask;
			this.syncRoot.Attributes |= MemberAttributes.Public;
			this.syncRoot.Type = CodeTypeReferenceStore.Get(typeof(object));
			this.syncRoot.HasGet = true;
			this.syncRoot.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(thisList, ICollectionSyncRootPropertyName)));
			this.syncRoot.HasSet = false;
			this.Members.Add(this.syncRoot);

			// public MyType[] ToArray()
			this.toArray = new CodeMemberMethod();
			this.toArray.Name = ToArrayMethodName;
			this.toArray.Attributes &= ~MemberAttributes.AccessMask;
			this.toArray.Attributes |= MemberAttributes.Public;
			this.toArray.ReturnType = arrayType;
			this.toArray.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(arrayType, new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(@this, BaseInnerListPropertyName), ArrayListToArrayPropertyName, new CodeTypeOfExpression(this.typeName)))));
			this.Members.Add(this.toArray);
			#endregion

			#region Events (Selective)
			if (events != CollectionEvents.None)
			{
				CodePatternUnaryOperatorExpression notInLoad = new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNot, new CodeMethodInvokeExpression(@this, this.beginEndLoad.IsInProcess.Name));
                
				if ((events & CollectionEvents.Clearing) != 0)
				{
					// public event System.EventHandler Clearing;
					this.clearing = new CodeMemberEvent();
					this.clearing.Attributes &= ~MemberAttributes.AccessMask;
					this.clearing.Attributes |= MemberAttributes.Public;
					this.clearing.Name = ClearingEventName;
					this.clearing.Type = CodeTypeReferenceStore.Get(typeof(EventHandler));
					this.Members.Add(this.clearing);

					// protected override void OnClear()
					this.onClearing = new CodeMemberMethod();
					this.onClearing.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
					this.onClearing.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
					this.onClearing.Name = BaseOnClearMethodName;
					this.onClearing.Statements.Add(
						new CodeConditionStatement(new CodeBinaryOperatorExpression(
						notInLoad,
						CodeBinaryOperatorType.BooleanAnd,
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, ClearingEventName))),
						new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, ClearingEventName), @this, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EventArgs)), EventArgsEmptyFieldName)))));
					this.Members.Add(this.onClearing);
				}

				if ((events & CollectionEvents.Cleared) != 0)
				{
					// public event System.EventHandler Cleared;
					this.cleared = new CodeMemberEvent();
					this.cleared.Attributes &= ~MemberAttributes.AccessMask;
					this.cleared.Attributes |= MemberAttributes.Public;
					this.cleared.Name = ClearedEventName;
					this.cleared.Type = CodeTypeReferenceStore.Get(typeof(EventHandler));
					this.Members.Add(this.cleared);

					// protected override void OnClearComplete()
					this.onCleared = new CodeMemberMethod();
					this.onCleared.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
					this.onCleared.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
					this.onCleared.Name = BaseOnClearCompleteMethodName;
					this.onCleared.Statements.Add(
						new CodeConditionStatement(new CodeBinaryOperatorExpression(
						notInLoad,
						CodeBinaryOperatorType.BooleanAnd,
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, ClearedEventName))),
						new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, ClearedEventName), @this, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EventArgs)), EventArgsEmptyFieldName)))));
					this.Members.Add(this.onCleared);
				}

				if ((events & (CollectionEvents.InsertionEvents | CollectionEvents.RemovalEvents)) != 0)
				{
					// public delegate void EventHandler(MyTypeCollection sender, MyTypeCollection.EventArgs e);
					this.eventHandler = new CodePatternDelegate(string.Empty,
						new CodeParameterDeclarationExpression(typeof(int), EventArgsIndexPropertyName),
						new CodeParameterDeclarationExpression(type, EventArgsValuePropertyName));
					this.eventHandler.SetComment(EventArgsIndexPropertyName, "The index of the value.");
					this.eventHandler.SetComment(EventArgsValuePropertyName, "The value raised by the event.");
					this.Members.AddRange(this.eventHandler);
					
					if ((events & CollectionEvents.Inserting) != 0)
					{
						// public event MyTypeCollection.EventHandler Inserting;
						this.inserting = new CodeMemberEvent();
						this.inserting.Attributes &= ~MemberAttributes.AccessMask;
						this.inserting.Attributes |= MemberAttributes.Public;
						this.inserting.Name = InsertingEventName;
						this.inserting.Type = CodeTypeReferenceStore.Get(this.eventHandler.Delegate.Name);
						this.Members.Add(this.inserting);

						// protected override void OnInsert(int index, object value)
						this.onInserting = new CodeMemberMethod();
						this.onInserting.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
						this.onInserting.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
						this.onInserting.Name = BaseOnInsertMethodName;
						this.onInserting.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
						this.onInserting.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), ValueVariableName));
						this.onInserting.Statements.Add(
							new CodeConditionStatement(new CodeBinaryOperatorExpression(
							notInLoad,
							CodeBinaryOperatorType.BooleanAnd,
							new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, InsertingEventName))),
							new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, InsertingEventName),
							@this,
							new CodeObjectCreateExpression(CodeTypeReferenceStore.Get(this.eventHandler.EventArgs.Name), new CodeVariableReferenceExpression(IndexVariableName), new CodeCastExpression(type, new CodeVariableReferenceExpression(ValueVariableName)))))));
						this.Members.Add(this.onInserting);
					}

					if ((events & CollectionEvents.Inserted) != 0)
					{
						// public event MyTypeCollection.EventHandler Inserted;
						this.inserted = new CodeMemberEvent();
						this.inserted.Attributes &= ~MemberAttributes.AccessMask;
						this.inserted.Attributes |= MemberAttributes.Public;
						this.inserted.Name = InsertedEventName;
						this.inserted.Type = CodeTypeReferenceStore.Get(this.eventHandler.Delegate.Name);
						this.Members.Add(this.inserted);

						// protected override void OnInsertComplete(int index, object value)
						this.onInserted = new CodeMemberMethod();
						this.onInserted.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
						this.onInserted.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
						this.onInserted.Name = BaseOnInsertCompleteMethodName;
						this.onInserted.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
						this.onInserted.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), ValueVariableName));
						this.onInserted.Statements.Add(
							new CodeConditionStatement(new CodeBinaryOperatorExpression(
							notInLoad,
							CodeBinaryOperatorType.BooleanAnd,
							new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, InsertedEventName))),
							new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, InsertedEventName),
							@this,
							new CodeObjectCreateExpression(CodeTypeReferenceStore.Get(this.eventHandler.EventArgs.Name), new CodeVariableReferenceExpression(IndexVariableName), new CodeCastExpression(type, new CodeVariableReferenceExpression(ValueVariableName)))))));
						this.Members.Add(this.onInserted);
					}
					
					if ((events & CollectionEvents.Removing) != 0)
					{
						// public event MyTypeCollection.EventHandler Removing;
						this.removing = new CodeMemberEvent();
						this.removing.Attributes &= ~MemberAttributes.AccessMask;
						this.removing.Attributes |= MemberAttributes.Public;
						this.removing.Name = RemovingEventName;
						this.removing.Type = CodeTypeReferenceStore.Get(this.eventHandler.Delegate.Name);
						this.Members.Add(this.removing);

						// protected override void OnRemove(int index, object value)
						this.onRemoving = new CodeMemberMethod();
						this.onRemoving.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
						this.onRemoving.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
						this.onRemoving.Name = BaseOnRemoveMethodName;
						this.onRemoving.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
						this.onRemoving.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), ValueVariableName));
						this.onRemoving.Statements.Add(
							new CodeConditionStatement(new CodeBinaryOperatorExpression(
							notInLoad,
							CodeBinaryOperatorType.BooleanAnd,
							new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, RemovingEventName))),
							new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, RemovingEventName),
							@this,
							new CodeObjectCreateExpression(CodeTypeReferenceStore.Get(this.eventHandler.EventArgs.Name), new CodeVariableReferenceExpression(IndexVariableName), new CodeCastExpression(type, new CodeVariableReferenceExpression(ValueVariableName)))))));
						this.Members.Add(this.onRemoving);
					}

					if ((events & CollectionEvents.Removed) != 0)
					{
						// public event MyTypeCollection.EventHandler Removed;
						this.removed = new CodeMemberEvent();
						this.removed.Attributes &= ~MemberAttributes.AccessMask;
						this.removed.Attributes |= MemberAttributes.Public;
						this.removed.Name = RemovedEventName;
						this.removed.Type = CodeTypeReferenceStore.Get(this.eventHandler.Delegate.Name);
						this.Members.Add(this.removed);

						// protected override void OnRemoveComplete(int index, object value)
						this.onRemoved = new CodeMemberMethod();
						this.onRemoved.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
						this.onRemoved.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
						this.onRemoved.Name = BaseOnRemoveCompleteMethodName;
						this.onRemoved.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
						this.onRemoved.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), ValueVariableName));
						this.onRemoved.Statements.Add(
							new CodeConditionStatement(new CodeBinaryOperatorExpression(
							notInLoad,
							CodeBinaryOperatorType.BooleanAnd,
							new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, RemovedEventName))),
							new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, RemovedEventName),
							@this,
							new CodeObjectCreateExpression(CodeTypeReferenceStore.Get(this.eventHandler.EventArgs.Name), new CodeVariableReferenceExpression(IndexVariableName), new CodeCastExpression(type, new CodeVariableReferenceExpression(ValueVariableName)))))));
						this.Members.Add(this.onRemoved);
					}
				}

				if ((events & CollectionEvents.SettingEvents) != 0)
				{
					// public delegate void SetEventHandler(MyTypeCollection sender, MyTypeCollection.SetEventArgs e);
					this.setEventHandler = new CodePatternDelegate(SetEventHandlerPrefix,
						new CodeParameterDeclarationExpression(typeof(int), EventArgsIndexPropertyName),
						new CodeParameterDeclarationExpression(type, EventArgsOldValuePropertyName),
						new CodeParameterDeclarationExpression(type, EventArgsNewValuePropertyName));
					this.setEventHandler.SetComment(EventArgsIndexPropertyName, "The zero-based index at which oldValue can be found.");
					this.setEventHandler.SetComment(EventArgsOldValuePropertyName, "The value to replace with newValue.");
					this.setEventHandler.SetComment(EventArgsNewValuePropertyName, "The new value of the element at index.");
					this.Members.AddRange(this.setEventHandler);

					if ((events & CollectionEvents.Setting) != 0)
					{
						// public event MyTypeCollection.SetEventHandler Setting;
						this.setting = new CodeMemberEvent();
						this.setting.Attributes &= ~MemberAttributes.AccessMask;
						this.setting.Attributes |= MemberAttributes.Public;
						this.setting.Name = SettingEventName;
						this.setting.Type = CodeTypeReferenceStore.Get(this.setEventHandler.Delegate.Name);
						this.Members.Add(this.setting);

						// protected override void OnSet(int index, object oldValue, object newValue)
						this.onSetting = new CodeMemberMethod();
						this.onSetting.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
						this.onSetting.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
						this.onSetting.Name = BaseOnSetMethodName;
						this.onSetting.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
						this.onSetting.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), OldValueVariableName));
						this.onSetting.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), NewValueVariableName));
						this.onSetting.Statements.Add(
							new CodeConditionStatement(new CodeBinaryOperatorExpression(
							notInLoad,
							CodeBinaryOperatorType.BooleanAnd,
							new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, SettingEventName))),
							new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, SettingEventName),
							@this,
							new CodeObjectCreateExpression(CodeTypeReferenceStore.Get(this.setEventHandler.EventArgs.Name), 
							new CodeVariableReferenceExpression(IndexVariableName),
							new CodeCastExpression(type, new CodeVariableReferenceExpression(OldValueVariableName)),
							new CodeCastExpression(type, new CodeVariableReferenceExpression(NewValueVariableName)))))));
						this.Members.Add(this.onSetting);
					}

					if ((events & CollectionEvents.Set) != 0)
					{
						// public event MyTypeCollection.SetEventHandler Set;
						this.set = new CodeMemberEvent();
						this.set.Attributes &= ~MemberAttributes.AccessMask;
						this.set.Attributes |= MemberAttributes.Public;
						this.set.Name = SetEventName;
						this.set.Type = CodeTypeReferenceStore.Get(this.setEventHandler.Delegate.Name);
						this.Members.Add(this.set);

						// protected override void OnSetComplete(int index, object oldValue, object newValue)
						this.onSet = new CodeMemberMethod();
						this.onSet.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
						this.onSet.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
						this.onSet.Name = BaseOnSetCompleteMethodName;
						this.onSet.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), IndexVariableName));
						this.onSet.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), OldValueVariableName));
						this.onSet.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), NewValueVariableName));
						this.onSet.Statements.Add(
							new CodeConditionStatement(new CodeBinaryOperatorExpression(
							notInLoad,
							CodeBinaryOperatorType.BooleanAnd,
							new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, SetEventName))),
							new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, SetEventName),
							@this,
							new CodeObjectCreateExpression(CodeTypeReferenceStore.Get(this.setEventHandler.EventArgs.Name), 
							new CodeVariableReferenceExpression(IndexVariableName),
							new CodeCastExpression(type, new CodeVariableReferenceExpression(OldValueVariableName)),
							new CodeCastExpression(type, new CodeVariableReferenceExpression(NewValueVariableName)))))));
						this.Members.Add(this.onSet);
					}
				}

				if ((events & CollectionEvents.Validating) != 0)
				{
					// public delegate void ValidationEventHandler(MyTypeCollection sender, MyTypeCollection.ValidationEventArgs e);
					this.validationEventHandler = new CodePatternDelegate(ValidationEventHandlerPrefix,
						new CodeParameterDeclarationExpression(type, EventArgsValuePropertyName));
					this.validationEventHandler.SetComment(EventArgsValuePropertyName, "The value to be validated.");
					this.Members.AddRange(this.validationEventHandler);

					// public event MyTypeCollection.ValidationEventHandler Validating;
					this.validating = new CodeMemberEvent();
					this.validating.Attributes &= ~MemberAttributes.AccessMask;
					this.validating.Attributes |= MemberAttributes.Public;
					this.validating.Name = ValidatingEventName;
					this.validating.Type = CodeTypeReferenceStore.Get(this.validationEventHandler.Delegate.Name);
					this.Members.Add(this.validating);

					// protected override void OnValidate(object value)
					this.onValidating = new CodeMemberMethod();
					this.onValidating.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
					this.onValidating.Attributes |= MemberAttributes.Family | MemberAttributes.Override;
					this.onValidating.Name = BaseOnValidateMethodName;
					this.onValidating.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), ValueVariableName));
					this.onValidating.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), BaseOnValidateMethodName, new CodeVariableReferenceExpression(ValueVariableName))));
					this.onValidating.Statements.Add(new CodePatternArgumentAssertIsInstanceOfStatement(ValueVariableName, type));
					this.onValidating.Statements.Add(
						new CodeConditionStatement(new CodeBinaryOperatorExpression(
						notInLoad,
						CodeBinaryOperatorType.BooleanAnd,
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, new CodeEventReferenceExpression(@this, ValidatingEventName))),
						new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(@this, ValidatingEventName),
						@this,
						new CodeObjectCreateExpression(CodeTypeReferenceStore.Get(this.validationEventHandler.EventArgs.Name), 
						new CodeCastExpression(type, new CodeVariableReferenceExpression(ValueVariableName)))))));
					this.Members.Add(this.onValidating);
				}
			}
			#endregion

			this.HasComments = true;
		}

		#region Constructor Overloads
		/// <summary>
		/// Initializes a new instance of the CodePatternTypedCollection class.
		/// </summary>
		/// <param name="type">The type of the instance to be stored in the collection.</param>
		public CodePatternTypedCollection(CodeTypeReference type)
			: this(type, CollectionEvents.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternTypedCollection class.
		/// </summary>
		/// <param name="type">The type of the instance to be stored in the collection.</param>
		public CodePatternTypedCollection(Type type)
			: this(type, CollectionEvents.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternTypedCollection class.
		/// </summary>
		/// <param name="type">The type of the instance to be stored in the collection.</param>
		public CodePatternTypedCollection(string type)
			: this(type, CollectionEvents.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternTypedCollection class.
		/// </summary>
		/// <param name="type">The type of the instance to be stored in the collection.</param>
		/// <param name="events">The events that the type will expose.</param>
		public CodePatternTypedCollection(CodeTypeReference type, CollectionEvents events)
			: this(type, type.BaseType.Substring(type.BaseType.LastIndexOf('.') + 1).Replace("[]", "Array"), events)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternTypedCollection class.
		/// </summary>
		/// <param name="type">The type of the instance to be stored in the collection.</param>
		/// <param name="events">The events that the type will expose.</param>
		public CodePatternTypedCollection(Type type, CollectionEvents events)
			: this(CodeTypeReferenceStore.Get(type), type.Name.Replace("[]", "Array"), events)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternTypedCollection class.
		/// </summary>
		/// <param name="type">The type of the instance to be stored in the collection.</param>
		/// <param name="events">The events that the type will expose.</param>
		public CodePatternTypedCollection(string type, CollectionEvents events)
			: this(CodeTypeReferenceStore.Get(type), type.Substring(type.LastIndexOf('.') + 1).Replace("[]", "Array"), events)
		{
		}
		#endregion

		#region Exposing Properties
		/// <summary>
		/// Gets the parameterless constructor.
		/// </summary>
		public CodeConstructor ParameterlessConstructor 
		{ 
			get 
			{ 
				return this.parameterlessConstructor;
			} 
		}

		/// <summary>
		/// Gets the constructor that takes an array of the type.
		/// </summary>
		public CodeConstructor ArrayConstructor 
		{
			get
			{
				return this.arrayConstructor; 
			} 
		}

		/// <summary>
		/// Gets the copy constructor.
		/// </summary>
		public CodeConstructor CollectionConstructor
		{ 
			get 
			{ 
				return this.collectionConstructor;
			} 
		}

		/// <summary>
		/// Gets the collection's indexer.
		/// </summary>
		public CodeMemberProperty Indexer
		{
			get
			{
				return this.indexer;
			} 
		}

		/// <summary>
		/// Gets the Insert method.
		/// </summary>
		public CodeMemberMethod Insert 
		{ 
			get
			{ 
				return this.insert;
			} 
		}

		/// <summary>
		/// Gets the Remove method.
		/// </summary>
		public CodeMemberMethod Remove 
		{ 
			get 
			{ 
				return this.remove;
			} 
		}

		/// <summary>
		/// Gets the Contains method.
		/// </summary>
		public CodeMemberMethod Contains
		{
			get 
			{ 
				return this.contains; 
			} 
		}

		/// <summary>
		/// Gets the IndexOf method.
		/// </summary>
		public CodeMemberMethod IndexOf 
		{
			get
			{
				return this.indexOf;
			}
		}

		/// <summary>
		/// Gets the Add method.
		/// </summary>
		public CodeMemberMethod Add 
		{ 
			get
			{ 
				return this.add; 
			}
		}

		/// <summary>
		/// Gets the AddRange method that takes an array.
		/// </summary>
		public CodeMemberMethod AddRangeArray
		{
			get 
			{
				return this.addRangeArray;
			}
		}

		/// <summary>
		/// Gets the AddRange method that takes a collection.
		/// </summary>
		public CodeMemberMethod AddRangeCollection 
		{
			get
			{
				return this.addRangeCollection;
			} 
		}

		/// <summary>
		/// Gets the CopyTo method.
		/// </summary>
		public CodeMemberMethod CopyTo
		{
			get 
			{
				return this.copyTo;
			} 
		}

		/// <summary>
		/// Gets the SyncRoot property.
		/// </summary>
		public CodeMemberProperty SyncRoot
		{
			get 
			{ 
				return this.syncRoot; 
			}
		}

		/// <summary>
		/// Gets the ToArray method.
		/// </summary>
		public CodeMemberMethod ToArray
		{
			get
			{ 
				return this.toArray; 
			} 
		}
		
		/// <summary>
		/// Gets the Begin/End pattern for loading.
		/// </summary>
		public CodePatternBeginEndProcess BeginEndLoad 
		{
			get
			{ 
				return this.beginEndLoad;
			}
		}

		/// <summary>
		/// Gets the EventHandler delegate used in the insertion and removal events.
		/// </summary>
		public CodePatternDelegate EventHandler
		{ 
			get
			{ 
				return this.eventHandler; 
			}
		}

		/// <summary>
		/// Gets the SetEventHandler delegate used in the Setting and Set events.
		/// </summary>
		public CodePatternDelegate SetEventHandler 
		{
			get
			{ 
				return this.setEventHandler; 
			} 
		}

		/// <summary>
		/// Gets the ValidationEventHandler delegate used in the Validating event.
		/// </summary>
		public CodePatternDelegate ValidationEventHandler 
		{
			get 
			{
				return this.validationEventHandler; 
			} 
		}

		/// <summary>
		/// Gets the Clearing event.
		/// </summary>
		public CodeMemberEvent Clearing
		{ 
			get 
			{
				return this.clearing;
			} 
		}

		/// <summary>
		/// Gets the Cleared event.
		/// </summary>
		public CodeMemberEvent Cleared
		{ 
			get
			{ 
				return this.cleared; 
			} 
		}

		/// <summary>
		/// Gets the Inserting event.
		/// </summary>
		public CodeMemberEvent Inserting 
		{
			get 
			{
				return this.inserting;
			} 
		}

		/// <summary>
		/// Gets the Inserted event.
		/// </summary>
		public CodeMemberEvent Inserted 
		{ 
			get
			{ 
				return this.inserted; 
			} 
		}

		/// <summary>
		/// Gets the Removing event.
		/// </summary>
		public CodeMemberEvent Removing 
		{ 
			get
			{ 
				return this.removing; 
			} 
		}

		/// <summary>
		/// Gets the Removed event.
		/// </summary>
		public CodeMemberEvent Removed
		{ 
			get
			{ 
				return this.removed;
			}
		}

		/// <summary>
		/// Gets the Setting event.
		/// </summary>
		public CodeMemberEvent Setting
		{
			get
			{ 
				return this.setting;
			}
		}

		/// <summary>
		/// Gets the Set event.
		/// </summary>
		public CodeMemberEvent Set
		{ 
			get
			{
				return this.set;
			} 
		}

		/// <summary>
		/// Gets the Validating event.
		/// </summary>
		public CodeMemberEvent Validating 
		{ 
			get
			{
				return this.validating;
			} 
		}

		/// <summary>
		/// Gets the OnClearing event raising method.
		/// </summary>
		public CodeMemberMethod OnClearing
		{
			get 
			{
				return this.onClearing;
			} 
		}

		/// <summary>
		/// Gets the OnCleared event raising method.
		/// </summary>
		public CodeMemberMethod OnCleared 
		{ 
			get 
			{
				return this.onCleared; 
			} 
		}

		/// <summary>
		/// Gets the OnInserting event raising method.
		/// </summary>
		public CodeMemberMethod OnInserting
		{
			get
			{ 
				return this.onInserting; 
			}
		}

		/// <summary>
		/// Gets the OnInserted event raising method.
		/// </summary>
		public CodeMemberMethod OnInserted
		{ 
			get
			{ 
				return this.onInserted;
			}
		}

		/// <summary>
		/// Gets the OnRemoving event raising method.
		/// </summary>
		public CodeMemberMethod OnRemoving
		{
			get 
			{ 
				return this.onRemoving; 
			} 
		}

		/// <summary>
		/// Gets the OnRemoved event raising method.
		/// </summary>
		public CodeMemberMethod OnRemoved
		{
			get 
			{ 
				return this.onRemoved;
			}
		}

		/// <summary>
		/// Gets the OnSetting event raising method.
		/// </summary>
		public CodeMemberMethod OnSetting 
		{ 
			get
			{ 
				return this.onSetting;
			} 
		}

		/// <summary>
		/// Gets the OnSet event raising method.
		/// </summary>
		public CodeMemberMethod OnSet
		{
			get
			{ 
				return this.onSet;
			} 
		}

		/// <summary>
		/// Gets the OnValidating event raising method.
		/// </summary>
		public CodeMemberMethod OnValidating 
		{ 
			get 
			{
				return this.onValidating;
			}
		}
		#endregion

		#region Comments
		/// <summary>
		/// Gets or sets whether the members generated by the pattern have comments.
		/// </summary>
		/// <value>Whether the members generated by the pattern have comments.</value>
		public bool HasComments
		{
			get
			{
				return (this.parameterlessConstructor.Comments.Count > 0);
			}
			set
			{
				this.Comments.Clear();
				this.parameterlessConstructor.Comments.Clear();
				this.arrayConstructor.Comments.Clear();
				this.collectionConstructor.Comments.Clear();
				this.indexer.Comments.Clear();
				this.insert.Comments.Clear();
				this.remove.Comments.Clear();
				this.contains.Comments.Clear();
				this.indexOf.Comments.Clear();
				this.add.Comments.Clear();
				this.addRangeArray.Comments.Clear();
				this.addRangeCollection.Comments.Clear();
				this.copyTo.Comments.Clear();
				this.syncRoot.Comments.Clear();
				this.toArray.Comments.Clear();

				if (value)
				{
					this.Comments.AddRange(new SummaryStatements("Represents a collection of " + new SeeExpression(this.typeName) + " elements."));
					this.parameterlessConstructor.Comments.AddRange(new CommentsForConstructor(this.Name));
					this.arrayConstructor.Comments.AddRange(new CommentsForConstructor(this.Name, 
						new ParameterStatements(ArrayVariableName, "A list of objects of type " + new SeeExpression(this.typeName) + " to initialize the collection with.")));
					this.collectionConstructor.Comments.AddRange(new CommentsForConstructor(this.Name, 
						new ParameterStatements(CollectionVariableName, "An object of type " + new SeeExpression(this.Name) + " containing objects to be copied into the new collection.")));

					this.indexer.Comments.AddRange(new CommentsForIndexer(PropertyAccessors.Both, "The " + new SeeExpression(this.typeName) + " at position " + new ParameterReferenceExpression(IndexVariableName),
						new ParameterStatements(IndexVariableName, "The position of the " + new SeeExpression(this.typeName))));

					this.insert.Comments.AddRange(new CommentsForMethod("Inserts an element into the collection at the specified index.",
						new ParameterStatements(IndexVariableName, "The zero-based index at which " + new ParameterReferenceExpression(ValueVariableName) + " should be inserted."),
						new ParameterStatements(ValueVariableName, "The " + new SeeExpression(this.typeName) + " to insert.")));
					this.insert.Comments.AddRange(new ExceptionStatements(typeof(ArgumentOutOfRangeException), new ParameterReferenceExpression(IndexVariableName) + " is less than zero or is greater than " + new SeeExpression(typeof(CollectionBase) + "." + ICollectionCountPropertyName)));
					this.remove.Comments.AddRange(new CommentsForMethod("Removes the first occurrence of a specific " + new SeeExpression(this.typeName) + " from the collection.",
						new ParameterStatements(ValueVariableName, "The " + new SeeExpression(this.typeName) + " to remove from the collection.")));
					this.contains.Comments.AddRange(new CommentsForMethod("Determines whether an element is in the collection.",
						"true if " + new ParameterReferenceExpression(ValueVariableName) + " is found in the collection; otherwise, false.",
						new ParameterStatements(ValueVariableName, "The " + new SeeExpression(this.typeName) + " to locate in the collection.")));
					this.indexOf.Comments.AddRange(new CommentsForMethod("Searches for the specified " + new SeeExpression(this.typeName) + " and returns the zero-based index of the first occurrence within the entire collection.",
						"The zero-based index of the first occurrence of " + new ParameterReferenceExpression(ValueVariableName) + " within the entire collection, if found; otherwise, -1.",
						new ParameterStatements(ValueVariableName, "The " + new SeeExpression(this.typeName) + " to locate in the collection.")));
					this.add.Comments.AddRange(new CommentsForMethod("Adds an object to the end of the collection.",
						"The collection index at which the " + new ParameterReferenceExpression(ValueVariableName) + " has been added.",
						new ParameterStatements(ValueVariableName, "The " + new SeeExpression(this.typeName) + " to be added to the end of the collection.")));
					this.addRangeArray.Comments.AddRange(new CommentsForMethod("Adds the elements of an array to the end of the collection.",
						new ParameterStatements(ArrayVariableName, "The array whose elements should be added to the end of the collection.")));
					this.addRangeArray.Comments.AddRange(new ExceptionStatements(typeof(ArgumentNullException), new ParameterReferenceExpression(ArrayVariableName) + " is a null reference (Nothing in Visual Basic)."));
					this.addRangeCollection.Comments.AddRange(new CommentsForMethod("Adds the elements of a collection to the end of the collection.",
						new ParameterStatements(CollectionVariableName, "The collection whose elements should be added to the end of the collection.")));
					this.addRangeCollection.Comments.AddRange(new ExceptionStatements(typeof(ArgumentNullException), new ParameterReferenceExpression(CollectionVariableName) + " is a null reference (Nothing in Visual Basic)."));
					this.copyTo.Comments.AddRange(new CommentsForMethod("Copies the entire collection to a compatible one-dimensional " + new SeeExpression(typeof(Array)) + ", starting at the specified " + new ParameterReferenceExpression(IndexVariableName) + " of the target array.",
						new ParameterStatements(SimpleArrayVariableName, "The one-dimensional " + new SeeExpression(typeof(Array)) + " that is the destination of the elements copied from collection. The " + new SeeExpression(typeof(Array)) + " must have zero-based indexing."),
						new ParameterStatements(IndexVariableName, "The zero-based index in " + new ParameterReferenceExpression(SimpleArrayVariableName) + " at which copying begins.")));
					this.copyTo.Comments.AddRange(new ExceptionStatements(typeof(ArgumentNullException), new ParameterReferenceExpression(SimpleArrayVariableName) + " is a null reference (Nothing in Visual Basic)."));
					this.copyTo.Comments.AddRange(new ExceptionStatements(typeof(ArgumentOutOfRangeException), new ParameterReferenceExpression(IndexVariableName) + " is less than zero."));
					this.copyTo.Comments.AddRange(new ExceptionStatements(typeof(ArgumentException), new ParameterReferenceExpression(SimpleArrayVariableName) + " is multidimensional."));
					this.copyTo.Comments.AddRange(new ExceptionStatements(typeof(ArgumentException), new ParameterReferenceExpression(IndexVariableName) + " is equal to or greater than the length of " + new ParameterReferenceExpression(SimpleArrayVariableName)));
					this.copyTo.Comments.AddRange(new ExceptionStatements(typeof(ArgumentException), "The number of elements in the source collection is greater than the available space from " + new ParameterReferenceExpression(IndexVariableName) + " to the end of the destination " + new ParameterReferenceExpression(SimpleArrayVariableName)));
					this.copyTo.Comments.AddRange(new RemarksStatements("The specified array's elements must be of type " + new SeeExpression(this.typeName) + " or any of its derivatives."));
					this.syncRoot.Comments.AddRange(new CommentsForProperty(PropertyAccessors.Get, "An object that can be used to synchronize access to the collection."));
					this.toArray.Comments.AddRange(new CommentsForMethod("Copies the elements of the collection to a new " + new SeeExpression(this.typeName) + " array.",
						"An array of " + new SeeExpression(this.typeName) + " containing copies of the elements of the collection."));
				}

				// Events
				if (this.beginEndLoad != null)
				{
					this.beginEndLoad.HasComments = value;

					if (this.clearing != null)
					{
						this.clearing.Comments.Clear();
						this.onClearing.Comments.Clear();

						if (value)
						{
							this.clearing.Comments.AddRange(new SummaryStatements("Occurs when the collection begins the process of removing all elements."));
							this.clearing.Comments.AddRange(new RemarksStatements("To indicate that clearing the collection is not possible, throw an exception from an attached method."));
							this.onClearing.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(ClearingEventName) + " event."));
						}
					}

					if (this.cleared != null)
					{
						this.cleared.Comments.Clear();
						this.onCleared.Comments.Clear();

						if (value)
						{
							this.cleared.Comments.AddRange(new SummaryStatements("Occurs after the collection completes the process of removing all elements."));
							this.onCleared.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(ClearedEventName) + " event."));
						}
					}

					if (this.eventHandler != null)
					{
						this.eventHandler.HasComments = value;

						if (this.inserting != null)
						{
							this.inserting.Comments.Clear();
							this.onInserting.Comments.Clear();

							if (value)
							{
								this.inserting.Comments.AddRange(new SummaryStatements("Occurs when the collection begins the process of inserting an element."));
								this.inserting.Comments.AddRange(new RemarksStatements("To indicate that inserting this element is invalid, throw an exception from an attached method. To indicate that the element is simply invalid, use the " + new SeeExpression(ValidatingEventName) + " event."));
								this.onInserting.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(InsertingEventName) + " event.",
									new ParameterStatements(IndexVariableName, "The zero-based index at which to insert " + new ParameterReferenceExpression(ValueVariableName)),
									new ParameterStatements(ValueVariableName, "The new value of the element at " + new ParameterReferenceExpression(IndexVariableName))));
							}
						}

						if (this.inserted != null)
						{
							this.inserted.Comments.Clear();
							this.onInserted.Comments.Clear();

							if (value)
							{
								this.inserted.Comments.AddRange(new SummaryStatements("Occurs after the collection completes the process of inserting an element."));
								this.inserted.Comments.AddRange(new RemarksStatements("To prevent the insertion from taking place, throw an exception from an attached method."));
								this.onInserted.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(InsertedEventName) + " event.",
									new ParameterStatements(IndexVariableName, "The zero-based index at which to insert " + new ParameterReferenceExpression(ValueVariableName)),
									new ParameterStatements(ValueVariableName, "The new value of the element at " + new ParameterReferenceExpression(IndexVariableName))));
							}
						}

						if (this.removing != null)
						{
							this.removing.Comments.Clear();
							this.onRemoving.Comments.Clear();

							if (value)
							{
								this.removing.Comments.AddRange(new SummaryStatements("Occurs when the collection begins the process of removing an element."));
								this.removing.Comments.AddRange(new RemarksStatements("To indicate that removing this element is invalid, throw an exception from an attached method. To indicate that the element is simply invalid, use the " + new SeeExpression(ValidatingEventName) + " event."));
								this.onRemoving.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(RemovingEventName) + " event.",
									new ParameterStatements(IndexVariableName, "The zero-based index at which " + new ParameterReferenceExpression(ValueVariableName) + " can be found."),
									new ParameterStatements(ValueVariableName, "The value of the element to remove from " + new ParameterReferenceExpression(IndexVariableName))));
							}
						}

						if (this.removed != null)
						{
							this.removed.Comments.Clear();
							this.onRemoved.Comments.Clear();
								
							if (value)
							{
								this.removed.Comments.AddRange(new SummaryStatements("Occurs after the collection completes the process of removing an element."));
								this.removed.Comments.AddRange(new RemarksStatements("To prevent the removal from taking place, throw an exception from an attached method."));
								this.onRemoved.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(RemovedEventName) + " event.",
									new ParameterStatements(IndexVariableName, "The zero-based index at which " + new ParameterReferenceExpression(ValueVariableName) + " can be found."),
									new ParameterStatements(ValueVariableName, "The value of the element to remove from " + new ParameterReferenceExpression(IndexVariableName))));
							}
						}
					}

					if (this.setEventHandler != null)
					{
						this.setEventHandler.HasComments = value;

						if (this.setting != null)
						{
							this.setting.Comments.Clear();
							this.onSetting.Comments.Clear();
								
							if (value)
							{
								this.setting.Comments.AddRange(new SummaryStatements("Occurs when the collection begins the process of setting an element at a certain position."));
								this.setting.Comments.AddRange(new RemarksStatements("To indicate that setting this element at this position is invalid, throw an exception from an attached method. To indicate that the element is simply invalid, use the " + new SeeExpression(ValidatingEventName) + " event."));
								this.onSetting.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(SettingEventName) + " event.",
									new ParameterStatements(IndexVariableName, "The zero-based index at which " + new ParameterReferenceExpression(OldValueVariableName) + " can be found."),
									new ParameterStatements(OldValueVariableName, "The value to replace with " + new ParameterReferenceExpression(NewValueVariableName)),
									new ParameterStatements(NewValueVariableName, "The new value of the element at " + new ParameterReferenceExpression(IndexVariableName))));
							}
						}

						if (this.set != null)
						{
							this.set.Comments.Clear();
							this.onSet.Comments.Clear();
													
							if (value)
							{
								this.set.Comments.AddRange(new SummaryStatements("Occurs after the collection completes the process of setting an element at a certain position."));
								this.set.Comments.AddRange(new RemarksStatements("To prevent the settng action from taking place, throw an exception from an attached method."));
								this.onSet.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(SetEventName) + " event.",
									new ParameterStatements(IndexVariableName, "The zero-based index at which " + new ParameterReferenceExpression(OldValueVariableName) + " can be found."),
									new ParameterStatements(OldValueVariableName, "The value to replace with " + new ParameterReferenceExpression(NewValueVariableName)),
									new ParameterStatements(NewValueVariableName, "The new value of the element at " + new ParameterReferenceExpression(IndexVariableName))));
							}
						}
					}

					if (this.validationEventHandler != null)
					{
						this.validationEventHandler.HasComments = value;
						this.validating.Comments.Clear();
						this.onValidating.Comments.Clear();

						if (value)
						{
							this.validating.Comments.AddRange(new SummaryStatements("Occurs when the collection asks for validation of an item that is to be added into it."));
							this.validating.Comments.AddRange(new RemarksStatements("If the item is invalid, throw an exception from an attached method." + Environment.NewLine +
								"Checks that already take place are that the value is not a null reference (Nothing in Visual Basic) and that it is of/derives from " + new SeeExpression(this.typeName)));
							this.onValidating.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(ValidatingEventName) + " event.",
								new ParameterStatements(ValueVariableName, "The object to validate.")));
						}
					}
				}
			}
		}
		#endregion
	}
}