mergeInto(LibraryManager.library, {

  Save: function (filename, text) {
		var element = document.createElement('a');
		element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(Pointer_stringify(text)));
		element.setAttribute('download', Pointer_stringify(filename));

		element.style.display = 'none';
		document.body.appendChild(element);

		element.click();

		document.body.removeChild(element);
	},
	
	ShowLoadingButton: function () {
		var element = document.getElementById("proxyButton");
		element.style.visibility = 'visible';
	}
  
});