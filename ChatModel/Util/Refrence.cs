using System;

namespace ChatModel.Util
{
	/// <summary>
	/// Generic util class for storing a reference to an object.
	/// </summary>
	/// <remarks>
	/// Basically a pointer to a pointer in C terms.
	/// </remarks>
	/// <typeparam name="T">Type to which a reference is stored.</typeparam>
	[Serializable]
	public class Refrence<T>
	{
		/// <summary>
		/// Reference to an object.
		/// </summary>
		public T Reference { get; set; }

		public Refrence(T reference) { Reference = reference; }

		//the following are some basic conversion operators
		public static implicit operator T(Refrence<T> x) { return x.Reference; }

		public static implicit operator Refrence<T>(T x) { return new Refrence<T>(x); }

		public static T operator ~(Refrence<T> x) { return x.Reference; }
	}
}

/*
This class's only responsibility is to allow for easier serialization, for which the "double reference" comes in handy. 
*/
