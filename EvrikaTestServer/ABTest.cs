using System;
using System.Collections.Generic;
using System.Linq;


public class ABTest
{
	public int id;
	public string name;
	public DateTime startDate = DateTime.MinValue;
	public DateTime endDate = DateTime.MaxValue;
	public string startCondition;
	public string endCondition;
	public Int64 usersCount;

	public List<ABGroup> groups = new List<ABGroup>()
	{
		new ABGroup(50),
		new ABGroup(50),
	};


	public void AddGroup()
	{
		var last = groups.Last();
		float value = last.value / 2;
		last.value = value;
		ABGroup group = new ABGroup(value);
		groups.Add(group);
	}
	public void RemoveGroup(ABGroup group)
	{
		if (groups.Count <= 2)
		{
			Console.WriteLine("The count of groups cannot be less than 2");
		}
		else
		{
			groups.Remove(group);
			float totalValue = groups.Sum(x => x.value);
			float ratioChange = 100f / totalValue;
			groups.ForEach(x => x.value *= ratioChange);
		}
	}
	public void RemoveGroup(int index)
	{
		if (groups.Count <= 2)
		{
			Console.WriteLine("The count of groups cannot be less than 2");
		}
		else
		{
			groups.RemoveAt(index);
			float totalValue = groups.Sum(x => x.value);
			float ratioChange = 100f / totalValue;
			groups.ForEach(x => x.value *= ratioChange);
		}
	}


	public void ChangeGroupValue(int index)
	{
		ABGroup group = groups[index];
		float value = group.value;
		if (value > 100)
		{
			Console.WriteLine("The value of group cannot be more than 100");
		}
		else
		{
			var others = groups.Except(new List<ABGroup>() { group }).ToList();
			float totalValue = others.Sum(x => x.value) + value;
			float ratioChange = 100 / totalValue;
			others.ForEach(x => x.value *= ratioChange);
		}
	}

	public int GetIndexGroup()
	{
		int index = -1;
		for (int i = 0; i < groups.Count; i++)
		{
			ABGroup? item = groups[i];
			if (item.value / 100f > item.countUsers / (float)usersCount)
			{
				return i;
			}
		}
		return index;
	}

}
