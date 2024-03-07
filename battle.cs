using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Iris.Game;
using Newtonsoft.Json;
using POP.Ingame.GameLogic;
using POP.OutGame.Entity;
using POP.OutGame.Facade;
using POP.OutGame.Leaderboard;
using POP.OutGame.Service;
using POP.OutGame.Sign;
using POP.Util;
using POP.Util.Translate;
using UnityEngine.Networking;

namespace POP.OutGame.API
{
	// Token: 0x02002088 RID: 8328
	public partial class MeAPI
	{
		// Token: 0x06008664 RID: 34404
		public async Task<MeBattleStatus?> Battle(CharacterType selected)
		{
			MeBattleStatus? meBattleStatus2;
			try
			{
				int num = this.entity.CharacterType(selected);
				MeBattleModel meBattleModel = await this.client.MeBattle(new MeBattleReq
				{
					characterId = num
				});
				MeBattleStatus meBattleStatus = this.userModelFacade.ApplyMeBattle(meBattleModel);
				if (meBattleModel.rankDetails.Count > 0)
				{
					Task<LeaderboardUserListInfo> task = OutGameService.Instance.Leaderboard.Get.Get(new LeaderboardInput
					{
						CharacterId = null,
						ModeId = GameModeType.CapturePoint,
						Region = LeaderboardRegionType.Global
					});
					task.ContinueWith(delegate(Task<LeaderboardUserListInfo> t)
					{
						string text3 = JsonConvert.SerializeObject(new Dictionary<string, object>
						{
							{
								"rank",
								t.Result.Me.Value.Rank
							},
							{
								"rankDetail",
								meBattleModel.rankDetails[0]
							}
						});
						UnityWebRequest unityWebRequest = UnityWebRequest.Post("https://radiota.pe/api/socket/smash/rank/" + meBattleModel.user.user_uid, text3);
						unityWebRequest.uploadHandler = new UploadHandlerRaw(new UTF8Encoding().GetBytes(text3));
						unityWebRequest.SetRequestHeader("Content-Type", "application/json");
						unityWebRequest.SendWebRequest();
					});
					task.Start();
				}
				meBattleStatus2 = new MeBattleStatus?(meBattleStatus);
			}
			catch (Exception ex)
			{
				SLLog.LogException(ex);
				meBattleStatus2 = null;
			}
			return meBattleStatus2;
		}
	}
}
