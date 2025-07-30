using Framework.Extensions;
using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class MatchCard : MonoBehaviour, IPoolable
    {
        public TMP_Text dateTime;
        public TMP_Text result;
        public TMP_Text status;
        public TMP_Text homeTeam;
        public TMP_Text awayTeam;
        public RawImage homeTeamLogo;
        public RawImage awayTeamLogo;

        public Fixture Fixture { get; set; }
        
        public void OnDespawn()
        {
            dateTime.text = string.Empty;
            result.text = string.Empty;
            status.text = string.Empty;
            homeTeam.text = string.Empty;
            awayTeam.text = string.Empty;
            homeTeamLogo.texture = null;
            awayTeamLogo.texture = null;
            Fixture = null;
        }
    }
}