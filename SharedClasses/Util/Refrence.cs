﻿using System;

namespace ChatModel.Util
{
	[Serializable]
	public class Refrence<T>
	{
		public T Reference { get; set; }

		public Refrence(T reference) { Reference = reference; }

		public static implicit operator T(Refrence<T> x) { return x.Reference; }

		public static implicit operator Refrence<T>(T x) { return new Refrence<T>(x); }

		public static T operator ~(Refrence<T> x) { return x.Reference; }
	}
}