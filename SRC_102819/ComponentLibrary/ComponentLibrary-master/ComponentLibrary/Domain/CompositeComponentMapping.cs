using System.Collections.Generic;
using System.Linq;

namespace TE.ComponentLibrary.ComponentLibrary.Domain
{
	public class CompositeComponentMapping
	{
		public CompositeComponentMapping()
		{
			ServiceColumnMapping = null;
			GroupCodeMapping = new Dictionary<string, string>();
		}

		public CompositeComponentMapping(Dictionary<string, Dictionary<string, string>> serviceColumnMapping, Dictionary<string, string> groupCodeMapping)
		{
			GroupCodeMapping = groupCodeMapping;
			ServiceColumnMapping = serviceColumnMapping;
		}

		public Dictionary<string, string> GroupCodeMapping { get; set; }

		public Dictionary<string, Dictionary<string, string>> ServiceColumnMapping { get; set; }

		public void AddColumnMapping(string headerKey, string sfgColumnKey, string serviceColumnKey)
		{
			if (!ServiceColumnMapping.ContainsKey(headerKey))
			{
				ServiceColumnMapping.Add(headerKey, new Dictionary<string, string>());
			}
			if (!ServiceColumnMapping[headerKey].ContainsKey(sfgColumnKey))
			{
				ServiceColumnMapping[headerKey][sfgColumnKey] = serviceColumnKey;
			}
		}

		public List<IHeaderData> MapServiceTodata(IService serviceData, ICompositeComponentDefinition sfgDefinition)
		{
			HeaderColumnData data = CreateEmptyObject(sfgDefinition);
			foreach (var key in ServiceColumnMapping.Keys)
			{
				var header = serviceData.Headers.SingleOrDefault(h => h.Key == key);
				if (header != null)
				{
					foreach (var columnKey in ServiceColumnMapping[key].Keys)
					{
						var serviceColumn = ServiceColumnMapping[key][columnKey];
						var column = header.Columns.SingleOrDefault(c => c.Key == serviceColumn);
						if (column != null)
						{
							data.Insert(key, columnKey, column.Value);
						}
					}
				}
			}
			return data.GetData();
		}

		private HeaderColumnData CreateEmptyObject(ICompositeComponentDefinition sfgDefinition)
		{
			var headerColumnData = new HeaderColumnData();
			foreach (var sfgDefinitionHeader in sfgDefinition.Headers)
			{
				var headerData = new HeaderData(sfgDefinitionHeader.Name, sfgDefinitionHeader.Name);
				foreach (var column in sfgDefinitionHeader.Columns)
				{
					headerColumnData.Insert(sfgDefinitionHeader.Key, column.Key, null);
				}
			}
			return headerColumnData;
		}
	}
}