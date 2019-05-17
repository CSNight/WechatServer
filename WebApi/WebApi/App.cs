namespace WebApi
{
	public static class App
	{
		public static string PYQContent = "<TimelineObject><id>0</id><username>{0}</username><createTime>0</createTime><contentDescShowType>0</contentDescShowType><contentDescScene>0</contentDescScene><private>0</private><contentDesc>{1}</contentDesc><contentattr>0</contentattr><sourceUserName></sourceUserName><sourceNickName></sourceNickName><statisticsData></statisticsData><ContentObject><contentStyle>1</contentStyle><title></title><description></description><contentUrl></contentUrl><mediaList>{2}</mediaList></ContentObject><actionInfo></actionInfo><appInfo><id></id></appInfo><location city=\"\" poiClassifyId=\"\" poiName=\"\" poiAddress=\"\" poiClassifyType=\"0\"></location><publicUserName></publicUserName></TimelineObject>";

		public static string EPYQContent = "<TimelineObject><id>0</id><username>{0}</username><createTime>0</createTime><contentDescShowType>0</contentDescShowType><contentDescScene>0</contentDescScene><private>0</private><contentDesc>%s</contentDesc><contentattr>0</contentattr><sourceUserName></sourceUserName><sourceNickName></sourceNickName><statisticsData></statisticsData><ContentObject><contentStyle>1</contentStyle><title></title><description></description><contentUrl></contentUrl><mediaList>{1}</mediaList></ContentObject><actionInfo></actionInfo><appInfo><id></id></appInfo><location city=\"\" poiClassifyId=\"\" poiName=\"\" poiAddress=\"\" poiClassifyType=\"0\"></location><publicUserName></publicUserName></TimelineObject>";

		public static string PYQContentImage = "<media><id>0</id><type>2</type><title></title><description></description><private>0</private><url type=\"1\">{0}</url><thumb type=\"1\">{1}</thumb><size totalSize=\"{2}\" height=\"{3}\" width=\"{4}\"></size></media>";

		public static string SnsLink = "<TimelineObject><id>0</id><username>{0}</username><createTime>0</createTime><contentDescShowType>0</contentDescShowType><contentDescScene>0</contentDescScene><private>0</private><contentDesc>%s</contentDesc><contentattr>0</contentattr><sourceUserName/><sourceNickName/><statisticsData/><weappInfo><appUserName/><pagePath/></weappInfo><canvasInfoXml/><ContentObject><contentStyle>3</contentStyle><title>%s</title><description>%s</description><contentUrl>{1}</contentUrl></ContentObject><actionInfo><appMsg><messageAction/></appMsg></actionInfo><appInfo><id/></appInfo><location/><publicUserName/></TimelineObject>";

		public static string AppMsgXml = "<appmsg appid=\"$appid$\" sdkver=\"sdkver\"><title>$title$</title><des>$des$</des><action>view</action><type>5</type><showtype>0</showtype><content></content><url>$url$</url><thumburl>$thumburl$</thumburl><lowurl></lowurl><appattach><totallen>0</totallen><attachid></attachid><fileext></fileext></appattach><extinfo></extinfo></appmsg>";

		public static string DeviceKey = "<softtype><k3>9.0.2</k3><k9>iPad</k9><k10>2</k10><k19>58BF17B5-2D8E-4BFB-A97E-38F1226F13F8</k19><k20>{0}</k20><k21>neihe_5GHz</k21><k22>(null)</k22><k24>{1}</k24><k33>\\345\\276\\256\\344\\277\\241</k33><k47>1</k47><k50>1</k50><k51>com.tencent.xin</k51><k54>iPad4,4</k54></softtype>";
	}
}
