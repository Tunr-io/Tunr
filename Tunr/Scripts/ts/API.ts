class API {
	private authentication: Authentication;
	constructor() {
		this.authentication = new Authentication();
	}

	public getAuthentication(): Authentication {
		return this.authentication;
	}
} 

class Authentication {
	private access_token: string;
	//private refresh_token: string;
	private expire_time: Date;

	public is_authenticated(): boolean {
		return (typeof this.access_token !== 'undefined' && this.expire_time.getTime() - (new Date()).getTime() > 0);
	}

	public auth_password(email: string, password: string): JQueryPromise<string> {
		var retval: JQueryDeferred<string> = $.Deferred();
		$.Deferred
		$.ajax("/Token", {
			type: "POST",
			dataType: "JSON",
			data: { grant_type: "password", username: email, password: password },
			success: (data) => {
				this.access_token = data.access_token;
				//this.refresh_token = data.refresh_token;
				this.expire_time = new Date(data[".expires"]);
				retval.resolve();
			},
			error: () => {
				retval.reject("Failure.");
			}
		});
		return retval.promise();
	}
}