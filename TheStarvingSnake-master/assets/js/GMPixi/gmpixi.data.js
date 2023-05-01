
/* global GMPixi */

GMPixi.Cookie = {};

GMPixi.Cookie.create = function(key, value, howLong) {
    var expires = "";
    if (GMPixi.checkType(howLong, Number)) {
        var date = new Date();
        date.setTime(date.getTime() + howLong);
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = key + "=" + value + expires + "; path=/";
    return GMPixi.Cookie.get(key);
};

GMPixi.Cookie.get = function(key, defaultValue, createDefault, howLong) {
    key = key + "=";
    var ca = document.cookie.split(';');
    for(var i=0;i < ca.length;i++) {
        var c = ca[i];
        while (c.charAt(0)===' ') c = c.substring(1,c.length);
        if (c.indexOf(key) === 0) return c.substring(key.length,c.length).replace("=", "");
    }
    
    if(!GMPixi.checkType(createDefault, Boolean)) createDefault = false;
    if(!GMPixi.checkType(defaultValue)) defaultValue = null;
    
    if(createDefault && defaultValue !== null) {
        GMPixi.Cookie.create(key, defaultValue, howLong);
        return defaultValue;
    }
    return null;
};

GMPixi.Cookie.delete = function(key) {
    return GMPixi.Cookie.create(key, "", -1);
};





