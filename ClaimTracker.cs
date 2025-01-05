using System.Text.Json;
using static StringCollection;

public class ClaimTracker
{
    private Dictionary<long, int> _userClaims;
    private DateTime _currentDay;
    public int ClaimCount = 16;
    private string jsonFilePath = jsonUserClaims;

    public ClaimTracker()
    {
        _userClaims = new Dictionary<long, int>();
        _currentDay = DateTime.UtcNow.Date;
    }

    public bool CanClaim(long userId)
    {
        RefreshClaims();
        return !_userClaims.ContainsKey(userId) || _userClaims[userId] < ClaimCount; 
    }

    public void IncrementClaim(long userId)
    {
        RefreshClaims();

        if (_userClaims.ContainsKey(userId))
            _userClaims[userId]++;
        else
            _userClaims[userId] = 1; 
    }

    private void RefreshClaims()
    {
        if (_currentDay != DateTime.UtcNow.Date)
        {
            _userClaims.Clear(); 
            _currentDay = DateTime.UtcNow.Date; 
        }
    }

    public int GetClaimCount(long userId)
    {
        if (_userClaims.TryGetValue(userId, out int claimCount))
            return claimCount;
        else
            return 0; 
    }

    public async Task SaveClaimsToJson()
    {
        var userClaimsData = new UserClaimsData
        {
            UserClaims = _userClaims,
            CurrentDay = _currentDay
        };

        var json = JsonSerializer.Serialize(userClaimsData);
        await File.WriteAllTextAsync(jsonFilePath, json);
    }

    public async Task LoadClaimsFromJson()
    {
        if (File.Exists(jsonFilePath))
        {
            var json = await File.ReadAllTextAsync(jsonFilePath);
            if (string.IsNullOrWhiteSpace(json) || json == "[]")
            {
                _userClaims = new Dictionary<long, int>();
                _currentDay = DateTime.UtcNow.Date;
                return;
            }
            var userClaimsData = await Task.Run(() => JsonSerializer.Deserialize<UserClaimsData>(json));
            if (userClaimsData is null) { return; }

            _userClaims = userClaimsData.UserClaims;
            _currentDay = userClaimsData.CurrentDay;
        }
    }

    private class UserClaimsData
    {
        public required Dictionary<long, int> UserClaims { get; set; }
        public DateTime CurrentDay { get; set; }
    }
}