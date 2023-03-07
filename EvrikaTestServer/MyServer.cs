using System.Data.SqlClient;
using System.Text.Json;
using EvrikaTestServer;
using System.Linq;

public static class MyServer
{
	private const string connection = "Data Source=den1.mssql7.gear.host;Persist Security Info=True;User ID=mydb71;Password=Kq60-!3LFkvA";
	private static JsonSerializerOptions options = new JsonSerializerOptions()
	{
		IncludeFields = true,
	};

	public static string GetNewUserID()
	{

		//try
		//{
		Int64 id = GenerateID();
		WriteNewUser(id);
		return id.ToString();
		//}
		//catch (Exception ex)
		//{
		//	Console.WriteLine(ex.Message);
		//	return "-1";
		//}
	}

	private static void WriteNewUser(Int64 id)
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql2 = "INSERT INTO Users (UserID) VALUES (@UserID);";
			using (SqlCommand cmd2 = new SqlCommand(sql2, sqlConnection))
			{
				cmd2.Parameters.AddWithValue("@UserID", id);
				cmd2.ExecuteNonQuery();
			}

		}
	}

	private static Int64 GenerateID()
	{
		Random random = new Random();
		return random.NextInt64(0,Int64.MaxValue);
	}

	public static List<ABTest> GetABTests()
	{
		List<ABTest> tests = new List<ABTest>();
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{

			sqlConnection.Open();
			string sql = "SELECT * FROM Tests";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{

						ABTest test = JsonSerializer.Deserialize<ABTest>(reader.GetString(2), options);
						if (test != null)
						{
							tests.Add(test);
						}
					}
				}
			}
		}
		return tests;
	}

	public static ABTest GetABTest(int id)
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{

			sqlConnection.Open();
			string sql = "SELECT * FROM Tests WHERE Id=@Id";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				cmd.Parameters.AddWithValue("Id", id);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						ABTest test = JsonSerializer.Deserialize<ABTest>(reader.GetString(2), options);
						if (test != null)
						{
							return test;
						}
						if (!reader.IsDBNull(3))
						{
							test.usersCount = reader.GetInt64(3);
						}
					}
				}
			}
		}
		return null;
	}
	public static int GetNewIdForTest()
	{
		int id = 0;
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql = "SELECT TOP 1 * FROM Tests ORDER BY Id DESC";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						id = reader.GetInt32(0) + 1;
					}
				}
			}
		}
		return id;
	}
	public static void CreateNewTest(ABTest test)
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{

			sqlConnection.Open();
			string sql2 = "INSERT INTO Tests (Id, Name, Json, UsersCount) VALUES (@Id, @Name, @Json, @UsersCount);";
			using (SqlCommand cmd2 = new SqlCommand(sql2, sqlConnection))
			{
				cmd2.Parameters.AddWithValue("@Id", test.id);
				cmd2.Parameters.AddWithValue("@Name", test.name);
				string json = JsonSerializer.Serialize(test, options);
				cmd2.Parameters.AddWithValue("@Json", json);
				cmd2.Parameters.AddWithValue("@UsersCount", test.usersCount);
				cmd2.ExecuteNonQuery();
				//Console.WriteLine(json);
			}
		}
	}
	public static void UpdateTest(ABTest test)
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{

			sqlConnection.Open();
			string sql2 = "UPDATE Tests SET Name=@Name, Json=@Json, UsersCount=@UsersCount WHERE Id=@Id";
			using (SqlCommand cmd2 = new SqlCommand(sql2, sqlConnection))
			{
				string json = JsonSerializer.Serialize(test, options);
				cmd2.Parameters.AddWithValue("@Json", json);
				cmd2.Parameters.AddWithValue("@Id", test.id);
				cmd2.Parameters.AddWithValue("@Name", test.name);
				cmd2.Parameters.AddWithValue("@UsersCount", test.usersCount);
				cmd2.ExecuteNonQuery();
			}
		}
	}
	public static void DeleteTest(int id)
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql = "DELETE FROM Tests WHERE Id=@Id";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.ExecuteNonQuery();
			}
		}
	}
	public static void DeleteGroup(int id, int index)
	{
		ABTest test = GetABTest(id);
		test.RemoveGroup(index);
		UpdateTest(test);
	}
	public static void AddGroup(int id)
	{
		ABTest test = GetABTest(id);
		test.AddGroup();
		UpdateTest(test);
	}
	public static void ChangeValueGroup(int id, int index)
	{
		ABTest test = GetABTest(id);
		test.ChangeGroupValue(index);
		UpdateTest(test);
	}
	public static string GetTests(Int64 userID)
	{
		UserTestForJson userTestForJson = new UserTestForJson();
		UserTests userTests = null;
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql = "SELECT * FROM Users WHERE UserId=@UserId";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				cmd.Parameters.AddWithValue("UserId", userID);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					reader.Read();
					userTests = JsonSerializer.Deserialize<UserTests>(reader.GetString(1), options);
				}
			}
			foreach (var item in userTests.abTests)
			{
				ABTest test = null;
				string sql2 = "SELECT * FROM Tests WHERE Id=@Id";
				using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
				{
					cmd.Parameters.AddWithValue("Id", item);
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							test = JsonSerializer.Deserialize<ABTest>(reader.GetString(2), options);
							userTestForJson.abTests.Add(test);
						}
					}
				}
			}
		}
		string json = JsonSerializer.Serialize(userTestForJson, options);
		return json;
	}
	public static void SetActiveTest(Int64 userID, int testid, int groupIndex)
	{
		UserTests userTests = null;
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql = "SELECT * FROM Users WHERE UserId=@UserId";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				cmd.Parameters.AddWithValue("UserId", userID);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					reader.Read();
					userTests = JsonSerializer.Deserialize<UserTests>(reader.GetString(1), options);
					if (!userTests.abTests.Any(x => x.id == testid))
					{
						userTests.abTests.Add(new UserTest() { id = testid, group = groupIndex });
					}
				}
			}
		}
	}
	public static List<User> GetUsers()
	{
		List<User> users = new List<User>();
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{

			sqlConnection.Open();
			string sql = "SELECT * FROM Users";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{

						User user = new User();
						user.id = reader.GetInt64(0);
						if (!reader.IsDBNull(1))
						{
							user.userTests = JsonSerializer.Deserialize<UserTests>(reader.GetString(1), options);
						}
						users.Add(user);

					}
				}
			}
		}
		return users;
	}
	public static void DeleteUser(int id)
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql = "DELETE FROM Users WHERE UserID=@UserID";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				cmd.Parameters.AddWithValue("@UserID", id);
				cmd.ExecuteNonQuery();
			}
		}
	}
	public static void DeleteAllUsers()
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql = "DELETE FROM Users";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				cmd.ExecuteNonQuery();
			}
		}
	}
	public static string GetTestss(HttpRequest request)
	{
		Console.WriteLine(Int64.Parse(request.Query["id"]));
		return "okok";
	}
	public static string GetServerParams(HttpRequest request)
	{
		ServerParams serverParams = new ServerParams();
		Int64 userID = Int64.Parse(request.Query["id"]);
		User user = new User();
		user.id = userID;
		Dictionary<int, ABTest> abTests = new Dictionary<int, ABTest>();

		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{
			sqlConnection.Open();
			string sql = "SELECT * FROM Users WHERE UserId=@UserId";
			using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
			{
				cmd.Parameters.AddWithValue("UserId", userID);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					reader.Read();

					if (!reader.IsDBNull(1))
					{
						user.userTests = JsonSerializer.Deserialize<UserTests>(reader.GetString(1), options);
					}
				}
			}
			if (user.userTests != null && user.userTests.abTests != null)
			{
				string sql2 = "SELECT * FROM Tests WHERE Id=@Id";
				foreach (var item in user.userTests.abTests)
				{
					using (SqlCommand cmd2 = new SqlCommand(sql2, sqlConnection))
					{
						cmd2.Parameters.AddWithValue("Id", item.id);
						using (SqlDataReader reader = cmd2.ExecuteReader())
						{
							reader.Read();

							if (!reader.IsDBNull(1))
							{
								ABTest test = JsonSerializer.Deserialize<ABTest>(reader.GetString(2), options);
								abTests.Add(item.id, test);
							}
						}
					}
				}
			}
			else
			{
				return "{}";
			}
			CombineServerParams(serverParams, user, abTests);
		}
		string json = JsonSerializer.Serialize(serverParams, options);
		return json;
	}

	private static void CombineServerParams(ServerParams serverParams, User user, Dictionary<int, ABTest> abTests)
	{
		foreach (var item in user.userTests.abTests)
		{
			if (item.group >= 0)
			{
				var tempServerParams = abTests[item.id].groups[item.group].serverParams;
				if (tempServerParams != null)
				{
					if (tempServerParams.freeMoney != -1)
					{
						serverParams.freeMoney = tempServerParams.freeMoney;
					}
					if (tempServerParams.expMyltyplier != -1)
					{
						serverParams.expMyltyplier = tempServerParams.expMyltyplier;
					}
					if (tempServerParams.enableADS != -1)
					{
						serverParams.enableADS = tempServerParams.enableADS;
					}
				}
			}
		}
	}

	public static void UpdateUser(User user)
	{
		using (SqlConnection sqlConnection = new SqlConnection(connection))
		{

			sqlConnection.Open();
			string sql = "UPDATE Users SET UserID=@UserID, TestsJson=@TestsJson WHERE UserID=@UserID";
			using (SqlCommand cmd2 = new SqlCommand(sql, sqlConnection))
			{
				cmd2.Parameters.AddWithValue("@UserID", user.id);
				string json = JsonSerializer.Serialize(user.userTests, options);
				cmd2.Parameters.AddWithValue("@TestsJson", json);
				cmd2.ExecuteNonQuery();
			}
		}
	}
	public static string GetAllTests()
	{
		var tests = GetABTests();
		var allTests = tests.Where(x => x.endDate > DateTime.UtcNow);
		AbTestsForSend abTestsForSend = new AbTestsForSend();
		foreach (var test in allTests)
		{
			AbTest testForSend = new AbTest(test);
			abTestsForSend.tests.Add(testForSend);
		}
		string json = JsonSerializer.Serialize(abTestsForSend, options);
		return json;
	}
	public static void SetTestToUser(Int64 userID, int testID)
	{
		ABTest test = GetABTest(testID);
		List<User> users = GetUsers();
		User user = users.First(x => x.id == userID);
		var usersWithTest = users.Where(x => x.userTests.abTests != null && x.userTests.abTests.Any(y => y.id == testID)).ToList();
		for (int i = 0; i < test.groups.Count; i++)
		{
			test.groups[i].countUsers = usersWithTest.Where(x => x.userTests.abTests.Any(y => y.id == testID && y.group == i)).Count();
		}
		int group = test.GetIndexGroup();
		if (user.userTests.abTests == null)
		{
			user.userTests.abTests = new List<UserTest>();
		}
		user.userTests.abTests.Add(new UserTest() { id = testID, group = group });
		test.usersCount++;
		UpdateTest(test);
		UpdateUser(user);
	}
	public static void SetTestsToUser(HttpRequest request)
	{
		Int64 userID = Int64.Parse(request.Query["id"]);
		string listIds = request.Query["list"];
		if (string.IsNullOrEmpty(listIds))
		{
			return;
		}
		var ids = listIds.Split(',');
		foreach (var item in ids)
		{
			SetTestToUser(userID, int.Parse(item));
		}
	}
}
