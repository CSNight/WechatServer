namespace WebApi.Model
{
	public class ApiServerMsg
	{
		public bool Success
		{
			get;
			set;
		}

		public object Context
		{
			get;
			set;
		}

		public string ErrContext
		{
			get;
			set;
		}
	}
}
