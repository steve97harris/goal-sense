using Framework.Extensions;
using Framework.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.Screens
{
    public class MatchView : MonoBehaviour
    {
        public TMP_Text dateTime;
        public TMP_Text result;
        public TMP_Text homeTeam;
        public TMP_Text awayTeam;
        public RawImage homeTeamLogo;
        public RawImage awayTeamLogo;

        public FixturesService.Fixture Fixture { get; set; }
    }
}