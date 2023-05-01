
/* global GMPixi, PIXI, Function */

/**
 * Creates an instance of room. 
 * @param {Object} details contains the data for the room
 * @returns {GMPixi.Room}
 */
GMPixi.Room = function(details) {
    PIXI.Container.call(this);
    
    //setting the room global details
    this.room = details.room;
    this.rooms = details.rooms;
    
    //parse methods first
    if(GMPixi.checkType(details.methods, Object)) {
        for(var imethod in details.methods) {
            //protection for reserve keys
            if(GMPixi.isOneOf(['update', 'reset', 'room', 'rooms', '_update', 
                '_reset', 'default'])) continue;
            this[imethod] = details.methods[imethod].bind(this);
        }
    } 
    
    //parse data, override methods if same key
    if(GMPixi.checkType(details.data, Object)) {
        for(var idata in details.data) {
            //protection for reserve keys
            if(GMPixi.isOneOf(['update', 'reset', 'room', 'rooms', '_update', 
                '_reset', 'default'])) continue;
            this[idata] = details.data[idata];
        }
    }
    
    //parse methods
    
    
    this.default = GMPixi.checkType(details.default, Boolean) 
            ? details.default : false;
    
    //the setup function
    if(GMPixi.checkType(details.setup, Function)) {
        details.setup.call(this);
    }
    
    //setting the update
    this.update = GMPixi.checkType(details.update, Function) 
            ? details.update.bind(this) : null;
            
    //setting the reset
    this.reset = GMPixi.checkType(details.reset, Function)
            ? details.reset.bind(this) : null;
    
};

GMPixi.Room.prototype = Object.create(PIXI.Container.prototype);

GMPixi.Room.prototype._update = function() {
    //do updates on children first
    for(var c in this.children) {
        if(GMPixi.checkType(this.children[c].update, Function)) {
            this.children[c].update();
        }
    }
    
    //do the defined update here
    if(this.update !== null) this.update();
};

GMPixi.Room.prototype._reset = function() {
    //do resets on children first
    for(var c in this.children) {
        if(GMPixi.checkType(this.children[c].reset, Function)) {
            this.children[c].reset();
        }
    }
    
    //do the defined update here
    if(this.reset !== null) this.reset();
};


GMPixi.Room.prototype.add = function(obj, x, y, ax, ay, sx, sy, rot, a) {
    if(!GMPixi.checkType(obj)) {
        return null;
    }
    
    //set defaults
    x = GMPixi.Math.toNumber(x, 0);
    y = GMPixi.Math.toNumber(y, 0);
    ax = GMPixi.Math.toNumber(ax, 0);
    sx = GMPixi.Math.toNumber(sx, 1);
    rot = GMPixi.Math.toNumber(rot, 0);
    a = GMPixi.Math.toNumber(a, 1);
    
    if(!GMPixi.isTypeOf(ay, [Number, String])) ay = ax;
    else ay = GMPixi.Math.toNumber(ay, 0);
    
    if(!GMPixi.isTypeOf(ay, [Number, String])) sy = sx;
    else sy = GMPixi.Math.toNumber(sy, 1);
    
    //set position
    obj.position.set(x, y);
    
    //set anchor
    if(GMPixi.checkType(obj.anchor)) obj.anchor.set(ax, ay);
    
    //set scales
    if(GMPixi.checkType(obj.scale)) {
        obj.scale.x = sx;
        obj.scale.y = sy;
    }
    
    //set rotation
    if(GMPixi.checkType(obj.rotation)) obj.rotation = rot;
    
    //set alpha/opacity
    if(GMPixi.checkType(obj.alpha)) obj.alpha = a;
    
    //add the object to the room
    this.addChild(obj);
    
    //return the obj for further use
    return obj;
};

GMPixi.Room.prototype.addContainer = function(x, y, px, py) {
    var obj = new PIXI.Container();
    
    //set defaults
    x = GMPixi.Math.toNumber(x, 0);
    y = GMPixi.Math.toNumber(y, 0);
    px = GMPixi.Math.toNumber(px, 0);
    
    
    if(!GMPixi.isTypeOf(py, [Number, String])) py = px;
    else py = GMPixi.Math.toNumber(py, 0);
    
    if(GMPixi.checkType(obj.pivot)) obj.pivot.set(px, py);
    this.addChild(obj);
    return obj;
};