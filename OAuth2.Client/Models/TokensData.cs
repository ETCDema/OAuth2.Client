using System.Collections.Specialized;
using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace OAuth2.Client.Models
{
	public class TokensData
	{
		private readonly Dictionary<string, string> _vals;

		public TokensData(JsonElement root)
		{
			_vals               = new();
			_add("", root);
		}

		public TokensData(IQueryCollection coll)
		{
			_vals               = new(coll.Keys.Count);
			foreach (string key in coll.Keys)
			{
				_vals.Add(key, coll[key].ToString());
			}
		}

		public TokensData(NameValueCollection coll)
		{
			_vals               = new(coll.Keys.Count);
			foreach (string key in coll.Keys)
			{
				_vals.Add(key, coll[key]!);
			}
		}

		public string? TryGet(string n)
		{
			return _vals.TryGetValue(n, out var val) ? val : null;
		}

		public TokensData Add(string n, string? v)
		{
			if (v!=null) _vals.Add(n, v);
			return this;
		}

		public string Get(string n)
		{
			return _vals.TryGetValue(n, out var val)
					? !string.IsNullOrEmpty(val)
					? val
					: throw new NullReferenceException(n)
					: throw new NullReferenceException(n);
		}

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
