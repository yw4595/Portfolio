let game;
window.addEventListener('load', function() {
    game = new GMPixi.Game({
        preroom: function() {
            //create the default previous score and high score
            this.room.global.highscore = GMPixi.Cookie.get('gm_tss_highscore', 0, true, 3600000 * 24 * 365);
            this.room.global.score = GMPixi.Cookie.get('gm_tss_score', 0, true, 3600000 * 24 * 365);
        },
        room: {
            renderer: 'canvas',
            position: 'center',
            width: 600,
            height: 400
        },
        rooms: {
            title: {
                default: true,
                setup: function() {
                    //title
                    this.add(new PIXI.Text("Snake!", {
                        fill: ['#aa0000', '#aaaa00', '#aaaaaa'],
                        fontFamily: "Century Gothic",
                        fontSize: 36,
                        fontWeight: 900,
                        fontStyle: 'italic',
                        stroke: '#4e4e4e',
                        strokeThickness: 4,
                        dropShadow: true,
                        dropShadowColor: '#2f2f2f',
                        dropShadowBlur: 4,
                        dropShadowAngle: Math.PI/16,
                        dropShadowDistance: 8
                    }), this.room.width/2, this.room.height*0.2, 0.5);
                    
                    //high score
                    let highscore = this.add(new PIXI.Text(0, {
                            fill: ['#ffff00', '#ccff00', '#ffcc00'],
                            fontFamily: "Century Gothic",
                            fontSize: 30,
                            stroke: '#4e4e4e',
                            strokeThickness: 3
                        }), this.room.width/2, this.room.height*0.4, 0.5);
                    
                    highscore.reset = function() {
                        highscore.text = "HighScore: " + this.room.global.highscore;
                    }.bind(this);
                    
                    //previous score
                    let pscore = this.add(new PIXI.Text(0, {
                            fill: '#ffff00',
                            fontFamily: "Century Gothic",
                            fontSize: 18,
                            stroke: '#4e4e4e',
                            strokeThickness: 3
                        }), this.room.width/2, this.room.height*0.5, 0.5);
                    
                    pscore.reset = function() {
                        pscore.text = "Previous Score: " + this.room.global.score;
                    }.bind(this);
                    
                    //levels
                    let diff = ['Easy', 'Medium', 'Hard'];
                    for(let i=0; i<diff.length; i++) {
                        let obj = this.add(new PIXI.Text(diff[i], {
                            fill: ['#ff00ff', '#ff88ff', '#ffffff'],
                            fontFamily: "Century Gothic",
                            fontSize: 25,
                            stroke: '#4e4e4e',
                            strokeThickness: 3
                        }), this.room.width/2, this.room.height * (0.65 + 0.1*i), 0.5);
                        
                        obj.interactive = true;
                        obj.buttonMode = true;
                        
                        obj.on('click', function() {
                            this.room.change('game', true, {
                                difficulty: i
                            });
                        }.bind(this)).on('mouseover', function() {
                            this.scale.x = 1.25;
                            this.scale.y = 1.25;
                        }.bind(obj)).on('mouseout', function() {
                            this.scale.x = 1;
                            this.scale.y = 1;
                        }.bind(obj));
                        
                        obj.reset = function() {
                            this.scale.x = 1;
                            this.scale.y = 1;
                        }.bind(obj);
                    }
                    
                    //if ever 'R' is pressed start with normal difficulty
                    window.addEventListener('keydown', function(e) {
                        if(this.room.current !== this.rooms['title']) return;
                        if(e.keyCode === 'R'.charCodeAt(0)) {
                            this.room.change('game', true, {
                                difficulty: 1
                            });
                        }
                    }.bind(this));
                }
            },
            game: {
                data: {
                    score: 0,
                    col_count: 24,
                    row_count: 18,
                    tile_size: new GMPixi.Vector.Dimension(12, 12),
                    snake_size: new GMPixi.Vector.Dimension(10, 10),
                    difficulty: 0,
                    current_dir: 0,
                    dir: {
                        horizontal: 0,
                        vertical: 1
                    }
                },
                methods: {
                    getX: function(c, r) {
                        return this.matrix[c][r].x;
                    },
                    getY: function(c, r) {
                        return this.matrix[c][r].y;
                    },
                    initSnake: function() {
                        
                        /**** Creating the HEAD ****/
                        //create the container that will hold the graphics
                        let part_cont = new PIXI.Container();
                        
                        //create the graphics drawer
                        let part = new PIXI.Graphics();
                        
                        //add the graphics to the container
                        part_cont.addChild(part);
                        
                        //do the drawing here
                        part.beginFill(0xffff00);
                        part.drawRect(0, 0, this.snake_size.w, 
                                this.snake_size.h);
                        part.endFill();
                        
                        //setting the position of the part_cont on the field
                        let ind_x = Math.floor(this.col_count/2);
                        let ind_y = Math.floor(this.row_count/2);
                        part_cont.position.set(this.getX(ind_x, ind_y) + 1, 
                                this.getY(ind_x, ind_y) + 1);
                        
                        //appending some properties needed
                       
                        part_cont.index = {
                            last: {
                                x: ind_x - 1,
                                y: ind_y
                            },
                            current: {
                                x: ind_x,
                                y: ind_y
                            },
                            next: {
                                x: ind_x + 1,
                                y: ind_y
                            }
                        };
                        
                        this.matrix[part_cont.index.current.x][part_cont.index.current.y].occupied = 1;
                        //adding the part_cont to the snake
                        this.snake.addChild(part_cont);
                        
                        /**** Creating the first tail ****/
                        let part_cont1 = new PIXI.Container();
                        let part1 = new PIXI.Graphics();
                        part_cont1.addChild(part1);
                        part1.beginFill(0x00ffcc);
                        part1.drawRect(0, 0, this.snake_size.w, 
                                this.snake_size.h);
                        part1.endFill();
                        part_cont1.position.set(
                                this.getX(ind_x-1, ind_y) + 1,
                                this.getY(ind_x-1, ind_y) + 1);
                        part_cont1.index = {
                            last: {
                                x: ind_x - 2,
                                y: ind_y
                            },
                            current: {
                                x: ind_x - 1,
                                y: ind_y
                            },
                            next: {
                                x: ind_x,
                                y: ind_y
                            }
                        };
                        this.matrix[part_cont1.index.current.x][part_cont1.index.current.y].occupied = 1;
                        this.snake.addChild(part_cont1);
                    },
                    appendSnake: function() {
                        //same as the first part of the initSnake()
                        let part_cont = new PIXI.Container();
                        let part = new PIXI.Graphics();
                        part_cont.addChild(part);
                        part.beginFill(0x00ffcc);
                        part.drawRect(0, 0, this.snake_size.w, 
                                this.snake_size.h);
                        part.endFill();
                        
                        //setting the position to last position of the last part
                        let lastPart = this.snake.children[this.snake.children.length - 1];
                        part_cont.index = {
                            last: { x: -1, y: -1 },
                            current: { x: lastPart.index.last.x, y: lastPart.index.last.y },
                            next: { x: lastPart.index.current.x, y: lastPart.index.current.y }
                        };
                        
                        part_cont.position.set(this.getX(part_cont.index.current.x, part_cont.index.current.y), 
                                this.getY(part_cont.index.current.x, part_cont.index.current.y));
                                
                        this.matrix[part_cont.index.current.x][part_cont.index.current.y].occupied = 1;
                        this.snake.addChild(part_cont);
                        
                    }
                },
                setup: function() {
                    
                    this.keys = {
                        up: false,
                        down: false,
                        left: false,
                        right: false
                    };
                    
                    /**** Creating the Matrix For Location ****/
                    //create the matrix
                    let by = this.room.height/2 - 6*this.row_count * 1.25;
                    let bx = this.room.width/2 - 6*this.col_count;
                    
                    this.matrix = [];
                    for(let c=0; c<this.col_count; ++c) {
                        this.matrix.push([]);
                        for(let r=0; r<this.row_count; ++r) {
                            //for the background tile
                            let tile = this.add(new PIXI.Graphics());
                            tile.beginFill( c%2 === 0 
                                    ? r%2===0 ? 0x2c2c2c : 0x3c3c3c
                                    : r%2 === 0 ? 0x3c3c3c : 0x2c2c2c);
                            tile.drawRect(bx + this.tile_size.w*c, by 
                                    + this.tile_size.h*r, this.tile_size.w, 
                                    this.tile_size.h);
                            tile.endFill();
                            
                            //for the positions
                            this.matrix[c].push(new GMPixi.Vector.Point(bx 
                                    + this.tile_size.w*c, by 
                                    + this.tile_size.h*r));
                            this.matrix[c][r].occupied = 0;
                        }
                    }
                    
                    /**** Creating the Snake Container ****/
                    //creating the snake container
                    this.snake = this.addContainer();
                    
                    //snake timer
                    this.snake.timer = {};
                    this.snake.timer.default = 0;
                    this.snake.timer.current = 0;
                    
                    //the snake reset function
                    this.snake.reset = function() {
                        this.snake.removeChildren();
                    }.bind(this);
                    
                    //what does the snake do every update
                    this.snake.update = function() {
                        //check if timer is elapsed
                        if(this.snake.timer.current-- > 0) return;
                        //reset timer
                        this.snake.timer.current = this.snake.timer.default;
                        
                        
                        let appendTail = false;
                        //do the shifting
                        let parts = this.snake.children;
                        for(let i=0; i<parts.length; ++i) {
                            
                            //setting the last and current
                            parts[i].index.last.x = parts[i].index.current.x;
                            parts[i].index.last.y = parts[i].index.current.y;
                            parts[i].index.current.x = parts[i].index.next.x;
                            parts[i].index.current.y = parts[i].index.next.y;
                            
                            //setting the next
                            //other parts
                            if(i > 0) {
                                //setting the last and current
                                parts[i].index.next.x = parts[i-1].index.current.x;
                                parts[i].index.next.y = parts[i-1].index.current.y;
                            }
                            //the head
                            else {
                                if(parts[i].index.last.x === parts[i].index.current.x) {   //vertical movement
                                    parts[i].index.next.x = parts[i].index.current.x;    //same x
                                    this.current_dir = this.dir.vertical;
                                    if(parts[i].index.last.y < parts[i].index.current.y) {  
                                        parts[i].index.next.y = parts[i].index.current.y + 1;
                                    }
                                    else {
                                        parts[i].index.next.y = parts[i].index.current.y - 1;
                                    }
                                }
                                else {              //horizontal movement
                                    parts[i].index.next.y = parts[i].index.current.y;    //same y
                                    this.current_dir = this.dir.horizontal;
                                    if(parts[i].index.last.x < parts[i].index.current.x) {  
                                        parts[i].index.next.x = parts[i].index.current.x + 1;
                                    }
                                    else {
                                        
                                        parts[i].index.next.x = parts[i].index.current.x - 1;
                                    }
                                }
                                
                                //check if change direction
                                if(this.current_dir === this.dir.horizontal) {  //do vertical shifts
                                    if(this.keys.up) {
                                        parts[i].index.next.x = parts[i].index.current.x;
                                        parts[i].index.next.y = parts[i].index.current.y - 1;
                                    }
                                    else if(this.keys.down){
                                        parts[i].index.next.x = parts[i].index.current.x;
                                        parts[i].index.next.y = parts[i].index.current.y + 1;
                                    }
                                }
                                else {
                                    if(this.keys.left) {
                                        parts[i].index.next.y = parts[i].index.current.y;
                                        parts[i].index.next.x = parts[i].index.current.x - 1;
                                    }
                                    else if(this.keys.right){
                                        parts[i].index.next.y = parts[i].index.current.y;
                                        parts[i].index.next.x = parts[i].index.current.x + 1;
                                    }
                                }
                                
                                
                                if(GMPixi.Math.isOnRange(parts[i].index.current.x, 0, this.col_count-1) &&
                                    GMPixi.Math.isOnRange(parts[i].index.current.y, 0, this.row_count-1)) {
                                    //eat food
                                    
                                    if(this.matrix[parts[i].index.current.x][parts[i].index.current.y].occupied === 2) {
                                        
                                        this.display.text = ++this.score; //adds the score the update display
                                        
                                        this.matrix[parts[i].index.current.x][parts[i].index.current.y].occupied = 1;
                                        let firstIndex = this.food.pointer;
                                        //move the food to another not-that-random location
                                        do {
                                            let pt = this.food.random[this.food.pointer];
                                            this.food.pointer = GMPixi.Math.next(this.food.pointer, 1, 0, this.food.random.length-1);
                                            //checks if the next location is occupied
                                            //or x and y is the 
                                            if(this.matrix[pt[0]][pt[1]].occupied === 0 && 
                                                    pt[0] !== parts[i].index.current.x && 
                                                    pt[1] !== parts[i].index.current.y) {

                                                //set food location
                                                this.food.x = this.getX(pt[0], pt[1]) + 2;
                                                this.food.y = this.getY(pt[0], pt[1]) + 2;
                                                this.food.index.x = pt[0];
                                                this.food.index.y = pt[1];
                                                this.matrix[pt[0]][pt[1]].occupied = 2;
                                                break;
                                            }
                                        } 
                                        while(firstIndex !== this.food.pointer);
                                        
                                        appendTail = true;      
                                        
                                        //when whole screen is covered
                                        if(firstIndex === this.food.pointer) {
                                            //game over
                                            this.room.change('score', true, {
                                                score: this.score
                                            });
                                        }
                                    }
                                }
                                
                            }
                            
                            
                            //end game
                            if(!GMPixi.Math.isOnRange(parts[i].index.current.x, 0, this.col_count-1) ||
                                    !GMPixi.Math.isOnRange(parts[i].index.current.y, 0, this.row_count-1)) {
                                this.room.change('score', true, {
                                    score: this.score
                                });
                                return;    
                            }
                            else {
                                //setting occupation if not yet game over
                                this.matrix[parts[i].index.current.x][parts[i].index.current.y].occupied = 1;
                                this.matrix[parts[i].index.last.x][parts[i].index.last.y].occupied = 0;
                            }
                            
                            //setting the current position
                            parts[i].x = this.getX(parts[i].index.current.x, parts[i].index.current.y) + 1;
                            parts[i].y = this.getY(parts[i].index.current.x, parts[i].index.current.y) + 1;
                            
                        }
                        
                        if(appendTail) {
                            this.appendSnake();
                        }
                        
                        //check if the head collide with other body parts
                        if(this.matrix[this.snake.children[0].index.current.x][this.snake.children[0].index.current.y].occupied === 1) {
                            //checks if other parts
                            let ind = [this.snake.children[0].index.current.x, this.snake.children[0].index.current.y];
                            for(let i=1; i<this.snake.children.length; ++i) {
                                if(this.snake.children[i].index.current.x === ind[0] && this.snake.children[i].index.current.y === ind[1]) {
                                    this.room.change('score', true, {
                                        score: this.score
                                    });
                                    break;
                                }
                            }
                            
                        }
                        
                    }.bind(this);
                    
                    //check for keys
                    window.addEventListener('keydown', function(e) {
                        //prevents this from happening if it is not the right room
                        if(this.room.current !== this.rooms['game']) return;
                        
                        if(e.keyCode === 37) this.keys.left = true;
                        
                        if(e.keyCode === 38) this.keys.up = true;
                        
                        if(e.keyCode === 39) this.keys.right = true;
                        
                        if(e.keyCode === 40) this.keys.down = true;
                        
                        //reset this room if R is pressed
                        if(e.keyCode === 'R'.charCodeAt(0)) {
                            this.room.change('game');
                        }
                        e.preventDefault();
                    }.bind(this));
                    
                    
                    window.addEventListener('keyup', function(e) {
                        //prevents this from happening if it is not the right room
                        if(this.room.current !== this.rooms['game']) return;
                        
                        if(e.keyCode === 37) this.keys.left = false;
                        
                        if(e.keyCode === 38) this.keys.up = false;
                        
                        if(e.keyCode === 39) this.keys.right = false;
                        
                        if(e.keyCode === 40) this.keys.down = false;
                        
                        e.preventDefault();
                    }.bind(this));
                    
                    /**** Creating the Food Container  ****/
                    
                    //almost the same with initSnake but arguments are different
                    this.food = this.addContainer();
                    let gfood = new PIXI.Graphics();
                    this.food.addChild(gfood);
                    gfood.beginFill(0xcc00cc);
                    gfood.drawRect(0, 0, 0.8 * this.snake_size.w, 0.8 * this.snake_size.h);
                    gfood.endFill();
                    
                    //init matrix loc index of the food
                    this.food.index = { x: 0, y: 0 };
                    
                    //the reset function
                    //randomize first position
                    this.food.reset = function() {
                        this.food.randomize();
                        let fx = GMPixi.Math.random(0, this.col_count);
                        let fr = Math.random() < 0.5;
                        let fy = GMPixi.Math.random(fr ? 0 : Math.floor(this.row_count/2) + 2, fr ? Math.floor(this.row_count/2) - 2 : this.row_count - 1);
                        this.food.position.set(this.getX(fx, fy) + 2, this.getY(fx, fy) + 2);
                        this.food.index.x = fx;
                        this.food.index.y = fy;
                    }.bind(this);
                    
                    //food is blinking
                    this.food.update = function() {
                        if(this.room.steps % 15 === 0) this.food.visible = !this.food.visible;
                    }.bind(this);
                    
                    //create the not-so-random array for next position locator
                    this.food.random = [];
                    for(let m in this.matrix) {
                        for(let n in this.matrix[m]) {
                            this.food.random.push([m, n]);
                        }
                    }
                    
                    //the pointer to the next location
                    this.food.pointer = 0;
                    
                    //randomize the not-so-random array for a bit good experience per game
                    this.food.randomize = function() {
                        this.food.random = GMPixi.Math.shuffleArray(this.food.random);
                    }.bind(this);
                    
                    
                    /**** Create the Point Display ****/
                    this.display = this.add(new PIXI.Text("0", {
                        fill: "#ffffff",
                        fontSize: 48,
                        fontFamily: "Century Gothic"
                    }), this.room.width/2, this.room.height * 0.9, 0.5);
                    
                    this.display.reset = function() {
                        this.text = "0";
                    }.bind(this.display);
                },
                reset: function() {
                    //create the initial snake
                    this.initSnake();
                    
                    //reset the score
                    this.score = 0;
                    
                    //reset all keys to false
                    for(let k in this.keys) this.keys[k] = false;
                    
                    //setting the def timer based on diff and reset the current
                    this.snake.timer.default = 7 - 2*this.difficulty;
                    this.snake.timer.current = this.snake.timer.default;
                    
                    //reset the matrix
                    for(let m in this.matrix) {
                        for(let n in this.matrix[m]) {
                            this.matrix[m][n].occupied = 0;
                        }
                    }
                    
                    //set matrix occupation of snake
                    for(let c in this.snake.children) {
                        let pt = this.snake.children[c].index.current;
                        this.matrix[pt.x][pt.y].occupied = 1;
                    }
                    
                    //set matrix occupation of the food
                    let pt = this.food.index;
                    this.matrix[pt.x][pt.y].occupied = 2;
                    
                    
                }
            },
            score: {
                data: {
                    score: 0
                },
                setup: function() {
                    //game over
                    this.add(new PIXI.Text("Game Over!", {
                        fill: ['#eeeeee'],
                        fontFamily: "Century Gothic",
                        fontSize: 32,
                        stroke: '#4e4e4e',
                        strokeThickness: 4
                    }), this.room.width/2, this.room.height * 0.2, 0.5);
                    
                    //the current score
                    let display = this.add(new PIXI.Text(0, {
                        fill: '#ffffff',
                        fontFamily: "Century Gothic",
                        fontSize: 24,
                        stroke: '#4e4e4e',
                        strokeThickness: 4
                    }), this.room.width/2, this.room.height * 0.35, 0.5);
                    
                    display.reset = function() {
                        display.text = "Your score: " + this.score;
                    }.bind(this);
                    
                    //the high score
                    let hs = this.add(new PIXI.Text("High Score", {
                        fill: '#ffff00',
                        fontFamily: "Century Gothic",
                        fontSize: 24,
                        stroke: '#4e4e4e',
                        strokeThickness: 4
                    }), this.room.width*0.5, this.room.height * 0.5, 0.6);
                    
                    hs.reset = function() {
                        hs.text = "High Score: " + this.room.global.highscore;
                        //change HS if the current score is the new high score
                        if(this.score > Number(this.room.global.highscore)) {
                            GMPixi.Cookie.create("gm_tss_highscore", this.score, 3600000 * 24 * 365);
                            this.room.global.highscore = GMPixi.Cookie.get("gm_tss_highscore", null);
                            if(GMPixi.Cookie.get("gm_tss_highscore", null) === null) {
                                this.room.global.highscore = this.score;
                            }
                        }
                    }.bind(this);
                    
                    //the previous score
                    let pscore = this.add(new PIXI.Text(0, {
                        fill: '#ff00ff',
                        fontFamily: "Century Gothic",
                        fontSize: 16,
                        stroke: '#4e4e4e',
                        strokeThickness: 4
                    }), this.room.width/2, this.room.height * 0.575, 0.5);
                    
                    pscore.reset = function() {
                        pscore.text = "Previous Score: " + this.room.global.score;
                        //change the previous score to the current score
                        GMPixi.Cookie.create("gm_tss_score", this.score, 3600000 * 24 * 365);
                        this.room.global.score = GMPixi.Cookie.get("gm_tss_score", null);
                        if(this.room.global.score === null) {
                            this.room.global.score = this.score;
                        }
                    }.bind(this);
                    
                    //the play again
                    let play =  this.add(new PIXI.Text("Play Again!", {
                        fill: '#00ff00',
                        fontFamily: "Century Gothic",
                        fontSize: 20,
                        stroke: '#4e4e4e',
                        strokeThickness: 4
                    }), this.room.width/4, this.room.height * 0.75, 0.5);
                    
                    play.reset = function() {
                        this.scale.x = 1;
                        this.scale.y = 1;
                    }.bind(play);
                    
                    play.interactive = true;
                    play.buttonMode = true;
                    play.on('click', function() {
                        this.room.change('game');
                    }.bind(this)).on('mouseover', function() {
                        this.scale.x = 1.25;
                        this.scale.y = 1.25;
                    }.bind(play)).on('mouseout', function() {
                        this.scale.x = 1;
                        this.scale.y = 1;
                    }.bind(play));
                    
                    
                    //the main menu
                    let menu =  this.add(new PIXI.Text("Main Menu!", {
                        fill: '#0033ff',
                        fontFamily: "Century Gothic",
                        fontSize: 20,
                        stroke: '#4e4e4e',
                        strokeThickness: 4
                    }), 3*this.room.width/4, this.room.height * 0.75, 0.5);
                    
                    menu.reset = function() {
                        this.scale.x = 1;
                        this.scale.y = 1;
                    }.bind(menu);
                    
                    menu.interactive = true;
                    menu.buttonMode = true;
                    menu.on('click', function() {
                        this.room.change('title');
                    }.bind(this)).on('mouseover', function() {
                        this.scale.x = 1.25;
                        this.scale.y = 1.25;
                    }.bind(menu)).on('mouseout', function() {
                        this.scale.x = 1;
                        this.scale.y = 1;
                    }.bind(menu));
                    
                    //if ever 'R' is pressed, start with previous difficulty
                    window.addEventListener('keydown', function(e) {
                        if(this.room.current !== this.rooms['score']) return;
                        if(e.keyCode === 'R'.charCodeAt(0)) {
                            this.room.change('game');
                        }
                    }.bind(this));
                    
                }
            }
        }
    });
});


