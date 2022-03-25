using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System.Web;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Pages
{
	public class IndexModel : PageModel
	{
		public AllGroups AllGroups = new AllGroups();
		public List<Group> CurrentGroups = new List<Group>();
		public CurrentPairs currentPairs = new CurrentPairs();
		public List<MyUser> UserList = new List<MyUser>();

		private readonly IMemoryCache _memoryCache;

		public IndexModel(IMemoryCache memoryCache) => _memoryCache = memoryCache;


		public PartialViewResult OnPost()
		{

			AllGroups = (AllGroups) _memoryCache.Get("AllGroups");
			currentPairs = (CurrentPairs) _memoryCache.Get("CurrentPairs");
			//currentPairs.Pairs[0].AddNext(AllGroups[2]);
			var TempViewData = new ViewDataDictionary<CurrentPairs>(ViewData, currentPairs);
			return new PartialViewResult()
			{
				ViewName = "_Pairs",
				ViewData = TempViewData,

			};

		}

		public PartialViewResult OnPostNewGroup(string Name)
		{
			AllGroups = (AllGroups) _memoryCache.Get("AllGroups");
			Group tempGroup = new Group();
			tempGroup.GroupUsers.Add(new MyUser() { Name = Name });
			AllGroups.AddGroup(tempGroup);
			var TempViewData = new ViewDataDictionary<AllGroups>(ViewData, AllGroups);
			return new PartialViewResult()
			{
				ViewName = "_Groups",
				ViewData = TempViewData,

			};
		}
		public PartialViewResult OnPostJoinGroup(string Name, string GroupID)
		{
			AllGroups = (AllGroups)_memoryCache.Get("AllGroups");
			int tempID = Int32.Parse(GroupID.Replace("group_", ""));
			Group tempGroup = AllGroups.Groups.Find(g => g.ID == tempID);
			tempGroup.GroupUsers.Add(new MyUser() { Name = Name });
			//AllGroups.AddGroup(tempGroup);
			var TempViewData = new ViewDataDictionary<AllGroups>(ViewData, AllGroups);
			return new PartialViewResult()
			{
				ViewName = "_Groups",
				ViewData = TempViewData,

			};
		}
		public PartialViewResult OnPostReady(string GroupID)
		{
			AllGroups = (AllGroups)_memoryCache.Get("AllGroups");
			currentPairs = (CurrentPairs)_memoryCache.Get("CurrentPairs");
			int tempID = Int32.Parse(GroupID.Replace("group_", ""));
			Group tempGroup = AllGroups.Groups.Find(g => g.ID == tempID);
			tempGroup.InQueue = true;
			currentPairs.Pairs[0].AddNext(tempGroup);
			//AllGroups.AddGroup(tempGroup);
			var TempViewData = new ViewDataDictionary<AllGroups>(ViewData, AllGroups);
			return new PartialViewResult()
			{
				ViewName = "_Groups",
				ViewData = TempViewData,

			};
			//var TempViewData2 = new ViewDataDictionary<CurrentPairs>(ViewData, currentPairs);
			//return new PartialViewResult()
			//{
			//	ViewName = "_Pairs",
			//	ViewData = TempViewData,

			//};

		}
		public void OnGet()
		{
			var groups = _memoryCache.TryGetValue("AllGroups", out AllGroups);
			if (AllGroups == null) {
				AllGroups = new AllGroups();
			Group tempGroup = new Group();
			tempGroup.GroupUsers.Add(new MyUser { Name = "Joe" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Dave" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Jen" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Steve" });
			AllGroups.AddGroup(tempGroup);

			tempGroup = new Group();
			tempGroup.GroupUsers.Add(new MyUser { Name = "Adam" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "PSim" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Abduul" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Nevermore" });
			AllGroups.AddGroup(tempGroup);


			tempGroup = new Group();
			tempGroup.GroupUsers.Add(new MyUser { Name = "Blue" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Rising" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Leky" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "Yamz" });
			AllGroups.AddGroup(tempGroup);

			tempGroup = new Group();
			tempGroup.GroupUsers.Add(new MyUser { Name = "User1" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "User2" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "User3" });
			tempGroup.GroupUsers.Add(new MyUser { Name = "User4" });
			AllGroups.AddGroup(tempGroup);
			_memoryCache.Set("AllGroups", AllGroups);
			currentPairs.Pairs.Add(new Pair(AllGroups.Groups[0], AllGroups.Groups[1]));
			currentPairs.Pairs[0].AddNext(AllGroups.Groups[2]);
			_memoryCache.Set("CurrentPairs", currentPairs);
				//currentPairs.Pairs[0].AddNext(AllGroups[3]);
			}
			else
			{
				AllGroups = (AllGroups)_memoryCache.Get("AllGroups");
				currentPairs = (CurrentPairs)_memoryCache.Get("CurrentPairs");
			}
		}
	}
}

public class MyUser
{
	public string Name { get; set; }
}

public class Group
{
	public List<MyUser> GroupUsers = new List<MyUser>();
	public bool InQueue = false;
	public int ID;
}

public class Pair
{
	public List<Group> Groups = new List<Group>();
	public List<Group> Next = new List<Group>();
	public Pair(Group g1, Group g2)
	{
		Groups.Add(g1);
		Groups.Add(g2);
		g1.InQueue = true;
		g2.InQueue = true;
	}
	public void AddNext(Group g1)
	{
		Next.Add(g1);
		g1.InQueue = true;
	}

}

public class CurrentPairs
{
	public List<Pair> Pairs = new List<Pair>();

}

public class AllGroups
{
	public List<Group> Groups = new List<Group>();
	public int AddGroup(Group g)
	{
		g.ID = Groups.Count();
		Groups.Add(g);
		return Groups.Count();
	}
}