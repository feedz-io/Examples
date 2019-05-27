// Import the Feedz.Client NuGet package

var name = "acme";
var orgName = "my-org";
var pat = ""; // Create a Personal Access Token for your user, used to create all the things

var client = FeedzClient.Create(pat);

var orgClient = client.ScopeToOrganisation(orgName);
var repository = await orgClient.Repositories.Create(new RepositoryCreateResource() {
	IsPublic = false,
	Name = name,
	Slug = Regex.Replace(name, "[^A-Za-z0-9]", "-").ToLower()
});

var team = await orgClient.Teams.Create(
	new TeamResource {
		Name = name,
		AllRepositories = false,
		Repositories = new List<TeamRepositoryResource> {
			new TeamRepositoryResource { Id = repository.Id }
		}
	}
);

var serviceAccount = await orgClient.ServiceAccounts.Create(
	new ServiceAccountResource
	{
		Name = name,
		Teams = new[] { new ServiceAccountTeamResource { TeamId = team.Id } }
	}
);

var accessTokenResponse = await client.PersonalAccessTokens.Create(
	new CreatePersonalAccessTokenRequest {
		Name = name,
		AccessLevel = PersonalAccessTokenResource.AccessLevelReadFeed,
		ServiceAccountId = serviceAccount.Id,
	}
);

Console.WriteLine(accessTokenResponse.Token);
