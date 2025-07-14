using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;

#if !MVC5
using Microsoft.AspNetCore.Http;
#endif

namespace OAuth2.Client.Models
{
	/// <summary>
	/// Adapter model for different token data sources
	/// </summary>
	public class TokensData
	{
		/// <summary>Key-value data</summary>
		private readonly Dictionary<string, string> _vals;

		/// <summary>
		/// From JSON source
		/// </summary>
		/// <param name="root">Source</param>
		public TokensData(JsonElement root)
		{
			_vals               = new();
			_add("", root);
		}

#if !MVC5
		/// <summary>
		/// From IQueryCollection source
		/// </summary>
		/// <param name="coll">Source</param>
		public TokensData(IQueryCollection coll)
		{
			_vals               = new(coll.Keys.Count);
			foreach (string key in coll.Keys)
			{
				_vals.Add(key, coll[key].ToString());
			}
		}
#endif

		/// <summary>
		/// From NameValueCollection source
		/// </summary>
		/// <param name="coll">Source</param>
		public TokensData(NameValueCollection coll)
		{
			_vals               = new(coll.Keys.Count);
			foreach (string key in coll.Keys)
			{
				_vals.Add(key, coll[key]!);
			}
		}

		/// <summary>
		/// Try get value by name
		/// </summary>
		/// <param name="n">Name of value</param>
		/// <returns>Value or null if not exists</returns>
		public string? TryGet(string n)
		{
			return _vals.TryGetValue(n, out var val) ? val : null;
		}

		/// <summary>
		/// Add named value into token
		/// </summary>
		/// <param name="n">Name</param>
		/// <param name="v">Value</param>
		/// <returns>This object for fluent-style code</returns>
		public TokensData Add(string n, string? v)
		{
			if (v!=null) _vals.Add(n, v);
			return this;
		}

		/// <summary>
		/// Get value by name
		/// </summary>
		/// <param name="n">Name of value</param>
		/// <returns>Value</returns>
		/// <exception cref="NullReferenceException">If value not exists or empty</exception>
		public string Get(string n)
		{
			return _vals.TryGetValue(n, out var val)
					? !string.IsNullOrEmpty(val)
					? val
					: throw new NullReferenceException(n)
					: throw new NullReferenceException(n);
		}

		/// <summary>
		/// Visit all nodes in el and add values with full path
		/// </summary>
		/// <param name="n">Elements prefix</param>
		/// <param name="el">Element for visit</param>
		private void _add(string n, JsonElement el)
		{
			switch (el.ValueKind)
			{
				case JsonValueKind.Undefined:
				case JsonValueKind.Null:
					break;

				case JsonValueKind.String:
					_vals.Add(n, el.GetString()!);
					break;

				case JsonValueKind.Number:
				case JsonValueKind.True:
				case JsonValueKind.False:
					_vals.Add(n, el.GetRawText()!);
					break;

				case JsonValueKind.Object:
					n           = n.Length>0 ? n+"." : n;
					var props	= el.EnumerateObject();
					while(props.MoveNext())
					{
						var p	= props.Current;
						_add(n+p.Name, p.Value);
					}
					break;
				case JsonValueKind.Array:
					n           = n.Length>0 ? n+"." : n;
					var els		= el.EnumerateArray();
					var i       = 0;
					while (els.MoveNext())
					{
						_add(n+(i++), els.Current);
					}
					break;
			}
		}
	}
}
