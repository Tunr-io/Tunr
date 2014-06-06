function htmlEscape(str) {
	return String(str)
		.replace(/&/g, '&amp;')
		.replace(/"/g, '&quot;')
		.replace(/'/g, '&#39;')
		.replace(/</g, '&lt;')
		.replace(/>/g, '&gt;');
}

function urlEscape(str: string): string {
	str = encodeURI(str);
	str = str.replace(/\//g, "-");
	str = str.replace(/:/g, "%20");
	str = str.replace(/\./g, "");
	str = str.replace(/&/g, "and");
	str = str.replace(/\+/g, " ");
	str = str.replace(/\?/g, " ");
	return str;
}