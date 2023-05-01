
/* global GMPixi */

/**
 * Creates the GMPixi.Math namespace
 * @type Object the functions of the GMPixi.Math
 */
GMPixi.Math = {};

/**
 * Convert an object to a number.
 * @param {Any} object the object to be converted to a number
 * @param {Number|String} defaultValue the fallback if converting fails, 
 * if defaultValue cannot be converted to number, will fallback to 0
 * @returns {Number} the converted number, if failed, the default value
 */
GMPixi.Math.toNumber = function(object, defaultValue) {
    if(!GMPixi.checkType(defaultValue, Number)) {
        defaultValue = Number(defaultValue);
        if(Number.isNaN(defaultValue)) defaultValue = 0;
    }
    object = Number(object);
    return Number.isNaN(object) ? defaultValue : object;
};

/**
 * The circumference of a unit circle. Use for circles.
 * @type Number Value of 2 pi.
 */
GMPixi.Math.Whole = 2 * Math.PI;

/**
 * Get a random number between the minimum and maximum value and return it as 
 * int or not if specified. May not work properly if min is greater than max.
 * @param {Number|String} min the floor range value(inclusive meaning included to the range), automatically converts to number, if failed, fallback to zero
 * @param {Number|String} max the ceiling range value(exclusive meaning not included to the range), automatically converts to number, if failed, fallback to 100
 * @param {Boolean} isInt if the result is an integer or not, if not defined, will return integer
 * @returns {Number} the random number generated as integer or float
 */
GMPixi.Math.random = function(min, max, isInt) {
    min = GMPixi.Math.toNumber(min, 0);
    max = GMPixi.Math.toNumber(max, 100);
    if(!GMPixi.checkType(isInt, Boolean)) isInt = true;
    var rand = Math.random() * (max - min) + min;
    return isInt ? Math.floor(rand) : rand;
};

/**
 * Makes a number be in between the min and max only.
 * @param {Number} val the number to be clamped
 * @param {Number} min the minimum value that can be attained 
 * (inclusive meaning included in the range)
 * @param {Number} max the maximum value that can be attained 
 * (inclusive meaning included in the range)
 * @returns {Number|undefined} the clamped value, or undefined if any input 
 * is not a number
 */
GMPixi.Math.clamp = function(val, min, max) {
    if(!GMPixi.checkType(val, Number)) {
        console.error("GMPixi.Math.clamp: value is not a number!");
        return;
    }
    if(!GMPixi.checkType(min, Number)) {
        console.error("GMPixi.Math.clamp: min is not a number!");
        return;
    }
    if(!GMPixi.checkType(max, Number)) {
        console.error("GMPixi.Math.clamp: max is not a number!");
        return;
    }

    return Number(min === max ? min : min < max ? 
        (val < min ? min : val > max ? max : val) : (val > max && val < min 
        ? Math.abs(min - val) > Math.abs(max - val) ? max : min : val));
};

/**
 * Checks if the value is inside the minimum and maximum value.
 * @param {Number} value the number to be checked
 * @param {Number} min the floor value, included in the range
 * @param {Number} max the ceiling value, included in the range
 * @returns {Boolean|undefined} true if value is inside the range, false if not,
 *  undefined if any input is undefined
 */
GMPixi.Math.isOnRange = function(value, min, max)  {
    if(!GMPixi.checkType(value, Number)) {
        console.error("GMPixi.Math.isOnRange: value is not a number!");
        return;
    }
    if(!GMPixi.checkType(min, Number)) {
        console.error("GMPixi.Math.isOnRange: min is not a number!");
        return;
    }
    if(!GMPixi.checkType(max, Number)) {
        console.error("GMPixi.Math.isOnRange: max is not a number!");
        return;
    }
    return min === max ? value === min : min < max ? value >= min 
            && value <= max : value >= max && value <= min;
};

/**
 * Makes the angle value inside the range of 0(inclusive) to 360(exclusive).
 * @param {Number} angle the angle value to be normalized
 * @returns {Number|undefined} the normalized angle, or undefined if the input angle is not a number
 */
GMPixi.Math.angleTo360 = function(angle) {
    if(!GMPixi.checkType(angle, Number)) {
        console.error("GMPixi.Math.angleTo360: angle is not a number!");
        return;
    }
    if(angle > 360) return angle % 360;
    while(angle < 0) {
        angle += 360;
    }
    return angle;
};

/**
 * Makes the angle value inside the range of 0(inclusive) and 2 pi(exclusive).
 * @param {Number} angle the angle to be normalized
 * @returns {Number|undefined} the normalized angle, or undefined if the input 
 * angle is not a number
 */
GMPixi.Math.angleTo2Pi = function(angle) {
    if(!GMPixi.checkType(angle, Number)) {
        console.error("GMPixi.Math.angleTo2Pi: angle is not a number!");
        return;
    }
    while(angle > GMPixi.Math.whole) angle -= GMPixi.Math.whole;
    while(angle < 0) angle += GMPixi.Math.whole;
    return angle;
};

/**
 * Converts degrees to radians.
 * @param {Number} deg the angle in degrees
 * @param {Number} normalized sets if angle will be normalized
 * @returns {Number|undefined} the angle converted to radians, 
 * or undefined if input angle is not a number
 */
GMPixi.Math.rad = function(deg, normalized) {
    if(!GMPixi.checkType(deg, Number)) {
        console.error("GMPixi.Math.rad: deg is not a number!");
        return;
    }
    if(!GMPixi.checkType(normalized, Boolean)) normalized = true;
    var angle = deg * Math.PI / 180;
    return normalized ? GMPixi.Math.angleTo2Pi(angle) : angle;
};

/**
 * Converts radians to degrees.
 * @param {Number} rad the angle in radians
 * @param {Number} normalized sets if angle will be normalized
 * @returns {Number|undefined} the angle converted to degrees, 
 * or undefined if input angle is not a number
 */
GMPixi.Math.deg = function(rad, normalized) {
    if(!GMPixi.checkType(rad, Number)) {
        console.error("GMPixi.Math.deg: rad is not a number!");
        return;
    }
    if(!GMPixi.checkType(normalized, Boolean)) normalized = true;
    var angle = rad * 180 / Math.PI;
    return normalized ? GMPixi.Math.angleTo360(angle) : angle;
};


GMPixi.Math.shuffleArray = function(array) {
    for (var i = array.length - 1; i > 0; i--) {
        var j = Math.floor(Math.random() * (i + 1));
        var temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
    return array;
};

GMPixi.Math.next = function(base, increment, min, max) {
    base += increment;
    return increment > 0 && base > max ?  min : 
            increment < 0 && base < min ?  max : base;
    
};