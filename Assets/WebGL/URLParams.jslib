mergeInto(LibraryManager.library, {

  GetQueryString: function () {
    var queryString = window.location.search;
    var bufferSize  = lengthBytesUTF8(queryString) + 1;
    var buffer      = _malloc(bufferSize);
    stringToUTF8(queryString, buffer, bufferSize);
    return buffer;
  }

});