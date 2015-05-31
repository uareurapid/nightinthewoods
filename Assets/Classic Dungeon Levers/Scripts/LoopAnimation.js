#pragma strict
var anim1 : String;
var anim2 : String;
var curAnim : int = 1;

function Start () {
	animation.Play(anim1);
	animation.wrapMode = WrapMode.Once;
}
function Update() {
	if(curAnim == 1) {
		if(!animation.isPlaying) {
			animation.CrossFade(anim2);
			animation.wrapMode = WrapMode.Once;
			curAnim = 2;
		}
	}
	else {
		if(curAnim == 2) {
			if(!animation.isPlaying) {
				animation.CrossFade(anim1);
				animation.wrapMode = WrapMode.Once;
				curAnim = 1;
			}
		}
	}
}