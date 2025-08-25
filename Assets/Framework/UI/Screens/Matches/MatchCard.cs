using Framework.Extensions;
using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class MatchCard : MonoBehaviour, IPoolable
    {
        public TMP_Text kickoffTime;
        public TMP_Text status;
        public TMP_Text result;
        public TMP_Text homeTeam;
        public TMP_Text awayTeam;
        public RawImage homeTeamLogo;
        public RawImage awayTeamLogo;

        [SerializeField] private Texture2D defaultLogo;

        public Fixture Fixture { get; set; }
        
        public void OnDespawn()
        {
            kickoffTime.text = string.Empty;
            result.text = string.Empty;
            status.text = string.Empty;
            homeTeam.text = string.Empty;
            awayTeam.text = string.Empty;
            homeTeamLogo.texture = defaultLogo;
            awayTeamLogo.texture = defaultLogo;
            Fixture = null;
        }

        public void Initialize(Fixture fixture)
        {
            Fixture = fixture;

            homeTeam.text = fixture.HomeTeam.ToFriendlyTeamName();
            awayTeam.text = fixture.AwayTeam.ToFriendlyTeamName();
            kickoffTime.text = fixture.Kickoff.ToString("h:mm tt").ToLower();
            kickoffTime.gameObject.SetActive(fixture.Status is "TIMED" or "SCHEDULED");
            status.text = fixture.GetStatusShort();
            status.gameObject.SetActive(fixture.Status is not ("TIMED" or "SCHEDULED"));
            result.text = $"{fixture.HomeScore} : {fixture.AwayScore}";
            result.gameObject.SetActive(fixture.Status is not ("TIMED" or "SCHEDULED"));
        }
    }
}