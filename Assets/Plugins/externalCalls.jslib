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
	
	ShowHtmlButtons: function () {
		var element = document.getElementById("proxyLoadButton");
		element.style.visibility = 'visible';
		element = document.getElementById("pasteButton");
		element.style.visibility = 'visible';
	}
  
});