﻿namespace ET.Client
{
	[MessageHandler(SceneType.Demo)]
	public class M2C_OperationHandler : MessageHandler<Scene, M2C_Operation>
	{
		protected override async ETTask Run(Scene root, M2C_Operation message)
        {
            Scene currentScene = root.CurrentScene();
            Log.Info($" operation back:{message?.OperateInfos?.Count}");
            if (message?.OperateInfos?.Count > 0)
            {
                Unit unit = UnitHelper.GetMyUnitFromClientScene(currentScene);
                foreach (OperateReplyInfo info in message.OperateInfos)
                {
                    EOperateType operateType = (EOperateType)info.OperateType;
                    switch (operateType)
                    {
                        case EOperateType.Skill1:
                            unit.GetComponent<SkillComponent>().SpellSkill(ESkillAbstractType.ActiveSkill);
                            break;
                        case EOperateType.Skill2:
                            unit.GetComponent<SkillComponent>().SpellSkill(ESkillAbstractType.ActiveSkill, 1);
                            break;
                    }
                }
            }
			await ETTask.CompletedTask;
		}
	}
}
