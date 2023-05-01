
/* global Function, PIXI */

/**
 * Initialize the GMPixi namespace
 * @returns {undefined} nothing
 */
function GMPixi() {}

/**
 * Checks whether the given object is a type of the given type.
 * @param {any} obj the object to be checked
 * @param {any} type the type that will be used, if undefined, only checks 
 * if the object is undefined
 * @returns {Boolean} true if the object is a type of the given type and false 
 * if not or is undefined
 */
GMPixi.checkType = function(obj, type) {
    try {
        if(typeof type === 'undefined') return typeof obj !== 'undefined';
        return obj.constructor === type;
    }
    catch(error) {
        return false;
    }
};

/**
 * Checks whether the obj is equal to value.
 * @param {Any} obj the object that need to be compared
 * @param {Any} value the value the object will be compared to 
 * @returns {Boolean} true, if object is equal to the value else false
 */
GMPixi.checkValue = function(obj, value) {
    try {
        return obj === value;
    }
    catch(error) {
        return false;
    }
};

/**
 * Check if the object is a type of types.
 * @param {Any} obj the object to be checked
 * @param {Any} types a type or array of types where obj will be compared
 * @returns {Boolean} true if it is a type or one of the type
 */
GMPixi.isTypeOf = function(obj, types) {
    if(!GMPixi.checkType(obj)) return false;
    if(GMPixi.checkType(types)) {
        if(GMPixi.checkType(types, Array)) {
            for(var i in types) {
                if(obj.constructor === types[i]) return true;
            }
        }
        else {
            return obj.constructor === types;
        }
    }
    return false;
};

GMPixi.isOneOf = function(obj, values) {
    if(GMPixi.checkType(obj)) return false;
    if(GMPixi.checkType(values)) {
        if(GMPixi.checkType(values, Array)) {
            for(var i in values) return obj === values[i];
        }
        else {
            return obj === values;
        }
    }
    return false;
};

/**
 * Gets the current point where the given html element is located.
 * @param {HTMLElement} el the element
 * @returns {GMPixi.Vector.point} the GMPixi.Vector.point location of the element
 */
GMPixi.getElementLocation = function(el) {
  var el = el.getBoundingClientRect();
  return GMPixi.Vector.point(el.left + window.scrollX, el.top + window.scrollY);
};

/**
 * Initialize a game class with options provided. 
 * @param {Object} o initial options in creating the game
 * @returns {GMPixi.Game} the game instance
 */
GMPixi.Game = function(o) {
    
    //checks if there options are defined else define an empty object
    if(!GMPixi.checkType(o, Object)) o = {};
    
    //checks if room options are defined else define an empty object
    if(!GMPixi.checkType(o.room, Object)) o.room = {};
    
    //intialize the room object
    this.room = {};
    this.rooms = {};
    
    //the width and height of the room
    this.room.width = Math.abs(GMPixi.Math.toNumber(o.room.width, 480));
    this.room.height = Math.abs(GMPixi.Math.toNumber(o.room.height, 320));
    
    //some inializations on the room properties
    this.room.steps = 0;        //counts the number of frame/loop that has been done
    this.room.global = {};      //storage for variables that will be used across rooms
    this.room.current = null;   //the current room rendered
    this.room.count = 0;        //total number of rooms created
    
    
    //creates the renderer
    switch(GMPixi.checkType(o.room.renderer, String) 
        ? o.room.renderer.toString().toLowerCase().replace(' ', '') 
        : 'auto') {
            case 'canvas': case 'c':
                this.renderer = new PIXI.CanvasRenderer(this.room.width, 
                        this.room.height, 
                        GMPixi.checkType(o.room.options, Object) 
                                ? o.room.options
                                : { background: '#000000' });
                break;
            case 'webgl': case 'gl': case 'wg': case 'w': case 'g':
                this.renderer = new PIXI.WebGLRenderer(this.room.width, 
                        this.room.height, 
                        GMPixi.checkType(o.room.options, Object) 
                                ? o.room.options
                                : { background: '#000000' });
                break;
            default: 
                this.renderer = PIXI.autoDetectRenderer(this.room.width, 
                        this.room.height, 
                        GMPixi.checkType(o.room.options, Object) 
                                ? o.room.options
                                : { background: '#000000' });
                break;
    }
    
    //the parent of the renderer's view
    if(GMPixi.checkType(o.room.parent, String)) {
        this.room.parent = document.getElementById(o.room.parent);
        if(this.room.parent === null) {
            this.room.parent = document.body;   
        }
    }
    else {
        this.room.parent = document.body;
    }
    
    this.room.parent.appendChild(this.renderer.view);
    
    //setting room current position on its parent
    if(GMPixi.checkType(o.room.position, String)) {
        var poskeys = ["left", "right", "top", "bottom", "topleft", "lefttop", 
                    "topright", "righttop", "bottomleft", "leftbottom", 
                    "bottomright", "rightbottom", 'center', 'middle', 'rt',
                    "tr", "br", "rb", "tl", "lt", "c", "m", "l", "r", "t", "b"];
        o.room.position = o.room.position.toString().toLowerCase()
                .replace('-', '').replace('_', '').replace(' ', '');
        if(poskeys.indexOf(o.room.position) <= -1) {
            o.room.position = "topleft";
        }
    }
    else if(GMPixi.checkType(o.room.position, Object)) {
        o.room.position.x = GMPixi.Math.toNumber(o.room.position.x, 0);
        o.room.position.y = GMPixi.Math.toNumber(o.room.position.y, 0);
    }
    else if(GMPixi.checkType(o.room.position, Array)) {
        o.room.position = {
            x: GMPixi.Math.toNumber(o.room.position[0], 0),
            y: GMPixi.Math.toNumber(o.room.position[1], 0)
        };
    }
    else {
        o.room.position = "topleft";
    }
    
    this.room.position = o.room.position;
    
    //use to relocate the canvas if ever the size of the window changes
    var relocateView = function() {
        this.room.parent.width = this.room.parent.scrollWidth;
        this.room.parent.height = this.room.parent.scrollHeight;
        
        switch(this.room.position) {
            case 'left': case 'l':
                this.room.x = 0;
                this.room.y = this.room.parent.height/2;
                break;
            case 'right': case 'r':
                this.room.x = this.room.parent.width - this.room.width;
                this.room.y = this.room.parent.height/2;
                break;
            case 'top': case 't':
                this.room.x = this.room.parent.width/2;
                this.room.y = 0;
                break;
            case 'bottom': case 'b':
                this.room.x = this.room.parent.width/2;
                this.room.y = this.room.parent.height - this.room.height;
                break;
            case 'topleft': case 'lefttop': case 'tl': case 'lt':
                this.room.x = 0;
                this.room.y = 0;
                break;        
            case 'topright': case 'righttop': case 'tr': case 'rt':
                this.room.x = this.room.parent.width - this.room.width;
                this.room.y = 0;
                break; 
            case 'bottomleft': case 'leftbottom': case 'bl': case 'lb':
                this.room.x = 0;
                this.room.y = this.room.parent.height - this.room.height;
                break;        
            case 'bottomright': case 'rightbottom': case 'br': case 'rb':
                this.room.x = this.room.parent.width - this.room.width;
                this.room.y = this.room.parent.height - this.room.height;;
                break; 
            case 'center': case 'middle': case 'c': case 'm':
                this.room.x = (this.room.parent.width - this.room.width)/2;
                this.room.y = (this.room.parent.height - this.room.height)/2;
                break;
        }
        this.renderer.view.style.left = this.room.x + "px";
        this.renderer.view.style.top = this.room.y + "px";
    }.bind(this);
    
    //if ever the window resizes or changes orientation
    window.addEventListener('resize', relocateView);
    relocateView();
    
    //some styling in the view
    this.renderer.view.style.position = "absolute";
    this.renderer.view.style.display = "block";
    
    //function before loading resources
    if(GMPixi.checkType(o.preload, Function)) {
        o.preload.call(this);
    }
    
    /*********************** FUNCTIONS HERE ***********************************/
    
    /**
    * Change the current room to the specified name. The may be reset and some
    * variable may overriden. The overriding of variables comes first before
    * calling reset. Calling this function in reset may not work well.
    * @param {String} rname the name of the destination room
    * @param {Boolean} reset if true, will call the reset function of the 
    * destination room
    * @param {Object} override the object containing the key pair that will 
    * overriden in the destination room
    * @returns {Boolean} true if destination room name is found and changed, 
    * may not work well if called on reset functions
    */
    this.room.change = function(rname, reset, override) {
        //checks if there exist a room named 'rname', if none, return false
        if(GMPixi.checkType(this.rooms[rname])) {
            
            //if reset is not defined, set it to true
            if(!GMPixi.checkType(reset, Boolean)) reset = true;
            
            //if override is not defined set it to empty objecct
            if(!GMPixi.checkType(override, Object)) override = {};

            //override the values of the keys in the room 
            for(var okey in override) {
                //prevents overriding of room and rooms
                if(okey === "room" || okey === "rooms") continue;
                this.rooms[rname][okey] = override[okey];
            }

            //reset if requested
            if(reset && GMPixi.checkType(this.rooms[rname]._reset, Function)) {
                this.rooms[rname]._reset();
            }
                

            //set the current room to the room requested
            this.room.current = this.rooms[rname];

            //if everything goes fine
            return true;
        }
        else {
            return false;
        }
    }.bind(this);
    
    /**
     * Creates a room with specified details.
     * @param {String} rname the name of the room, will not add room if ever
     * room name already exists
     * @param {Object} rdetails the details of the room, may contain setup,
     * update, reset, data and methods
     * return {Object|null} the room the has been created or null if 
     * unsuccessful
     */
    this.room.add = function(rname, rdetails) {
        
        //return false if no room name is specified
        if(!GMPixi.checkType(rname, String)) {
            return null;
        }
        
        //check if the given name already exists
        //if exists return false
        for(var rkey in this.rooms) {
            if(rkey === rname) return null;
        }
        
        //check if the given room details is an object
        //if not return false
        if(!GMPixi.checkType(rdetails, Object)) {
            return null;
        }
        
        //adding the pointer of this.room and this.rooms to the rdetails so 
        //that it can be accessed by the rooms
        rdetails.room = this.room;
        rdetails.rooms = this.rooms;
        
        
        //create a new GMPixi.Room with the specified detail and store it to
        //the rooms[rkey]
        this.rooms[rname] = new GMPixi.Room(rdetails);
        
        //increment the room count if adding was a success
        this.room.count++;
        return this.rooms[rname];
    }.bind(this);
    
    /*********************** END OF FUNCTIONS *********************************/
    
    
    //the setup function
    var setup = function() {
        //do something before creating the rooms
        if(GMPixi.checkType(o.preroom, Function)) {
            o.preroom.call(this);
        }
        
        //create the rooms
        if(GMPixi.checkType(o.rooms, Object)) {
            var firstRoom = null;       //tracks which room is first created
            var defaultRoom = null;     //tracks which room is default
            for(var roomname in o.rooms) {
                var room_obj = this.room.add(roomname, o.rooms[roomname]);
                if(room_obj.default && defaultRoom === null) defaultRoom = roomname;
                if(firstRoom === null) firstRoom = roomname;
            }
            
            //do post loading here
            if(GMPixi.checkType(o.postload, Function)) {
                o.postload.call(this);
            }
            
            //set the current room
            if(defaultRoom !== null) {
                this.room.change(defaultRoom);
            }
            else {
                if(firstRoom !== null) this.room.change(firstRoom);
            }
            
            //do the update after some delay
            setTimeout(this.update.bind(this), 3);
            
        }
        else {
            //do post loading if there is no room to be loaded
            if(GMPixi.checkType(o.postload, Function)) {
                o.postload.call(this);
            }
        }
        
        
        
    }.bind(this);
    
    //check for resource loading
    if(GMPixi.checkType(o.resource, Array)) {
        for(var i in o.resource) {
            PIXI.loader.add(o.resource[i]);
        }
        PIXI.loader.on('load', function(loader, resource) {
            if(GMPixi.checkType(o.onload, Function)) {
                o.onload.call(this, loader, resource);
            }
        }.bind(this)).load(setup);
    }
    else if(GMPixi.checkType(o.resource, String)) {
        PIXI.loader.add(o.resource)
                .on('load', function(loader, resource) {
                    if(GMPixi.checkType(o.onload, Function)) {
                        o.onload.call(this, loader, resource);
                    }
                }.bind(this)).load(setup);
    }
    else {
        setup();
    }
    
};

GMPixi.Game.prototype.update = function() {
    if(this.room.current !== null) {
        if(GMPixi.checkType(this.room.current._update, Function)) {
            this.room.current._update();
        }
        this.renderer.render(this.room.current);
    }
    this.room.steps++;
    requestAnimationFrame(this.update.bind(this));
};