

/* global GMPixi */

/**
 * Creates the GMPixi.Vector namespace.
 * @type Object the functions of the GMPixi.Vector.
 */        
GMPixi.Vector = {};

/**
 * Creates an instance of point object that stores x and y values.
 * @param {Number|String} ix the value of the x
 * @param {Number|String} iy the value of the y
 * @returns {GMPixi.Vector.Point} the Vector point object
 */
GMPixi.Vector.Point = function(ix, iy) {
    this.x = GMPixi.Math.toNumber(ix, 0);
    this.y = GMPixi.Math.toNumber(iy, 0);
};

/**
 * Creates an instance of a dimension object that stores w/width and 
 * h/height values. Will automatically converts inputs to number and get their 
 * absolute value, fallback is zero.
 * @param {Number} iw the width of the dimension
 * @param {Number} ih the height of the dimension
 * @returns {GMPixi.Vector.dimension} the Vector dimension object
 */
GMPixi.Vector.Dimension = function(iw, ih) {
    this.w = Math.abs(GMPixi.Math.toNumber(iw));
    this.h = Math.abs(GMPixi.Math.toNumber(ih));
    this.width = this.w;
    this.height = this.h;
};

/**
 * An alias of GMPixi.Vector.Dimension.
 * @param {Number} iw the width of the dimension
 * @param {Number} ih the height of the dimension
 * @returns {GMPixi.Vector.Dimension} the Vector dimension object
 */
GMPixi.Vector.dim = function(iw, ih) {
    return new GMPixi.Vector.Dimension(iw, ih);
};

/**
 * Creates a rectangle instance with specified position (x, y) as the top left 
 * corner and extend by the width to the right and height downward. Also 
 * contain the area, perimeter of the rectangle, the center point and the 
 * corner points. The corner point 'a' is the top left corner, 'b' is the top 
 * right, 'c' is the bottom right and 'd' is the bottom left. All values will 
 * be converted to number, fallback is zero.
 * @param {type} ix the x position of the rectangle
 * @param {type} iy the y position of the rectangle
 * @param {type} iw the width of the rectangle
 * @param {type} ih the height of the rectangle
 * @returns {GMPixi.Vector.Rectangle} the rectangle instance
 */
GMPixi.Vector.Rectangle = function(ix, iy, iw, ih) {
    this.point = GMPixi.Vector.point(ix, iy);
    this.dimension = GMPixi.Vector.dimension(iw, ih);
    this.dim = this.dimension;
    this.x = this.point.x;
    this.y = this.point.y;
    this.w = this.dim.w;
    this.h = this.dim.h;
    this.width = this.dim.w;
    this.height = this.dim.h;
    this.perimeter = (this.w + this.h) * 2;
    this.area = this.w * this.h;
    this.center = GMPixi.Vector.point(this.x + this.w/2, this.y + this.h/2);
    this.corner = {
        a: GMPixi.Vector.point(this.x, this.y),
        b: GMPixi.Vector.point(this.x + this.w, this.y),
        c: GMPixi.Vector.point(this.x + this.w, this.y + this.h),
        d: GMPixi.Vector.point(this.x, this.y + this.h)
    };
};

/**
 * An alias of GMPixi.Vector.rectangle.
 * @param {type} ix the x position of the rectangle
 * @param {type} iy the y position of the rectangle
 * @param {type} iw the width of the rectangle
 * @param {type} ih the height of the rectangle
 * @returns {GMPixi.Vector.Rectangle} the rectangle instance
 */
GMPixi.Vector.rect = function(ix, iy, iw, ih) {
    return new GMPixi.Vector.Rectangle(ix, iy, iw, ih);
};

