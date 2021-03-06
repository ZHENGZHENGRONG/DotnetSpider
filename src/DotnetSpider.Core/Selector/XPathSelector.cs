﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DotnetSpider.Core.Selector
{
	/// <summary>
	/// Xpath 查询器
	/// </summary>
	public class XPathSelector : BaseHtmlSelector
	{
		private static readonly Regex AttributeXPathRegex = new Regex(@"@[\w\s-]+", RegexOptions.RightToLeft | RegexOptions.IgnoreCase);
		private readonly string _xpath;
		private readonly string _attribute;

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="xpath">Xpath表达式</param>
		public XPathSelector(string xpath)
		{
			_xpath = xpath;

			Match match = AttributeXPathRegex.Match(_xpath);
			if (!string.IsNullOrEmpty(match.Value) && _xpath.EndsWith(match.Value))
			{
				_attribute = match.Value.Replace("@", "");
				_xpath = _xpath.Replace("/" + match.Value, "");
			}
		}

		/// <summary>
		/// 对节点进行查询, 查询结果为第一个符合查询条件的元素
		/// </summary>
		/// <param name="element">HTML元素</param>
		/// <returns>查询结果</returns>
		public override dynamic Select(HtmlNode element)
		{
			var node = element.SelectSingleNode(_xpath);
			if (node != null)
			{
				if (HasAttribute())
				{
					return node.Attributes.Contains(_attribute) ? node.Attributes[_attribute].Value?.Trim() : null;
				}
				else
				{
					return node;
				}
			}
			return null;
		}

		/// <summary>
		/// 对节点进行查询, 查询结果为所有符合查询条件的元素
		/// </summary>
		/// <param name="element">HTML元素</param>
		/// <returns>查询结果</returns>
		public override List<dynamic> SelectList(HtmlNode element)
		{
			List<dynamic> result = new List<dynamic>();
			var nodes = element.SelectNodes(_xpath);
			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					if (!HasAttribute())
					{
						result.Add(node);
					}
					else
					{
						var attr = node.Attributes[_attribute];
						if (attr != null)
						{
							result.Add(attr.Value?.Trim());
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 判断查询是否包含属性
		/// </summary>
		/// <returns>如果返回 True, 则说明是查询元素的属性值</returns>
		public override bool HasAttribute()
		{
			return !string.IsNullOrEmpty(_attribute);
		}
	}
}
