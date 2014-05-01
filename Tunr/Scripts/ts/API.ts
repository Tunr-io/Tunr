class API {
	private authentication: Authentication;
	constructor() {
		this.authentication = new Authentication();
	}

	public getAuthentication(): Authentication {
		return this.authentication;
	}

	public get(url: string): JQueryPromise<any> {
		var retval: JQueryDeferred<any> = $.Deferred<any>();
		$.ajax("/api/" + url, {
			type: "GET",
			dataType: "JSON",
			headers: { "Authorization": "Bearer " + this.authentication.get_access_token() },
			success: (data) => {
				retval.resolve(data);
			},
			error: () => {
				retval.reject();
			}
		});
		return retval.promise();
	}

	public post(url: string, data: any): JQueryPromise<any> {
		var retval: JQueryDeferred<any> = $.Deferred<any>();
		$.ajax("/api/" + url, {
			type: "POST",
			dataType: "JSON",
			data: data,
			headers: { "Authorization": "Bearer " + this.authentication.get_access_token() },
			success: (d) => {
				retval.resolve(d);
			},
			error: () => {
				retval.reject();
			}
		});
		return retval.promise();
	}
} 

class Authentication {
	private access_token: string;
	//private refresh_token: string;
	private expire_time: Date;

	public get_access_token(): string {
		return this.access_token;
	}

	public is_authenticated(): boolean {
		return (typeof this.access_token !== 'undefined' && this.expire_time.getTime() - (new Date()).getTime() > 0);
	}

	public auth_password(email: string, password: string): JQueryPromise<string> {
		var retval: JQueryDeferred<string> = $.Deferred();
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