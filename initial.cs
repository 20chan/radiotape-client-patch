using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Iris.Game;
using Newtonsoft.Json;
using POP.EnumType;
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
		// Token: 0x06008663 RID: 34403
		public async Task<bool> Initial(LangCultureType lang)
		{
			bool flag;
			try
			{
				string text = EnumTypeUtil<LangCultureType, LangCultureTypeUtil>.ToSlug(lang);
				MeInitialReq req = new MeInitialReq
				{
					langCulture = text
				};
				await this.preference.Update();
				var resp = await this.client.MeInitialFull(req);
				this.userModelFacade.ApplyMeInitial(resp);
				this.outGameStore.IsInitialized.Value = true;
				flag = true;
				string text2 = JsonConvert.SerializeObject(resp.user);
				UnityWebRequest unityWebRequest = UnityWebRequest.Post("https://radiota.pe/api/socket/smash/auth/" + resp.user.user_uid, text2);
				unityWebRequest.uploadHandler = new UploadHandlerRaw(new UTF8Encoding().GetBytes(text2));
				unityWebRequest.SetRequestHeader("Content-Type", "application/json");
				unityWebRequest.SendWebRequest();
				if (resp.rankDetails.Count > 0)
				{
					var task = OutGameService.Instance.Leaderboard.Get.Get(new LeaderboardInput
						{
							CharacterId = null,
							ModeId = null,
							Region = LeaderboardRegionType.Global
						}
					);
					task.ContinueWith(t => {
							string text3 = JsonConvert.SerializeObject(new Dictionary<string, object>
							{
								{
									"rank",
									t.Result.Me.Value.Rank
								},
								{
									"rankDetail",
									resp.rankDetails[0]
								}
							});
							UnityWebRequest unityWebRequest2 = UnityWebRequest.Post("https://radiota.pe/api/socket/smash/rank/" + resp.user.user_uid, text3);
							unityWebRequest2.uploadHandler = new UploadHandlerRaw(new UTF8Encoding().GetBytes(text3));
							unityWebRequest2.SetRequestHeader("Content-Type", "application/json");
							unityWebRequest2.SendWebRequest();
					});
					task.Start();
				}
			}
			catch (Exception ex)
			{
				SLLog.LogException(ex);
				flag = false;
			}
			return flag;
		}
	}
}
