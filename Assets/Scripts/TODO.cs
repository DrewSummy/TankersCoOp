/*

improve moving shooting ai
delegate certain traits to ai tanks
idea: when player in sight stop moving or rotating and wait an amount of time then shoot
    improve by still rotating
    improve by not charging
idea: have a function that repeatedly calls a move type and shoot type after a certain delta.time 
patrol points
idea: items to buy
    extra bullet
    map reveal
    point doubler for level
    extra life to respawn in start room
    idea: to increase effeciency dont run update on roomManagers, instead let enemy tanks have an instance of the room and decrement number of enemies with a function that once reaches 0 ends room

make buttons at pause do something
start on the correct pause/gameover button
save scores

take in multiple controller inputs
create second player

create inbetween level idles
save/continue/quit

create menu
include tutorial

create tutorial
learn cut scenes

improve aesthetics
better sounds (vocal maybe)
better art
make levels appear in creative ways
create mini-map (late update)
create explosion from last killed tank and push bullets players away
give player tanks have slightly different look than ai, maybe black outline
flashing red UI image over entire canvas when player dies/loses


idea:
    minigame: pushbattle where bullets don't kill they just push (damage multiplier dictates how far you get pushed)
    minigame: players get hit and a hole is placed where the player got hit so that the player would fall through
    minigame: race where bullets stun competitors
    bullet types: left trigger shoots slow, right- fast, both bidirectional bullets?
    teleport tank
    




Benchmarks:
    -1/20/17:
        contact a graphic designer (3D tank model, block textures, background, uiblocks, uipause, uisliders)
        contact a music designer (death, menu, battle)
    -2/20/17:
        make multiplayer
        add customization
    -3/20/17:
        ai
        create tutorial
    -4/20/17:
        minigames
        add sound effects
        give credit to https://github.com/daemon3000/InputManager/wiki/Getting-Started
*/
