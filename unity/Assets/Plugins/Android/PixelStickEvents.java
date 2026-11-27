package com.elswordpixel.input;

import android.app.Activity;
import android.view.MotionEvent;
import android.view.SurfaceView;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Proxy;
import java.util.Arrays;

/** Mirrors only the active game stick. Unity's original Window.Callback still receives every event. */
public final class PixelStickEvents {
    private static final int POINT=0, LIFT=1, CANCEL=2;
    private static final long[] EMPTY=new long[0];
    private static final long[] queue=new long[256*4];
    private static final int[] surfaceOrigin=new int[2];
    private static int used, finger=-1, generation;
    private static Window observed;
    private static Window.Callback delegate, wrapper;
    private static View surface;
    private static final View.OnAttachStateChangeListener attachment=new View.OnAttachStateChangeListener(){
        @Override public void onViewAttachedToWindow(View view) {}
        @Override public void onViewDetachedFromWindow(View view){
            synchronized(PixelStickEvents.class){if(surface==view){disarm();surface=null;}}
            view.removeOnAttachStateChangeListener(this);
        }
    };
    private static boolean armed,focused,resetPending;
    private static float left,top,right,bottom;

    private PixelStickEvents() {}

    public static void install(Activity activity) {
        if(activity!=null)activity.runOnUiThread(()->installOnUiThread(activity));
    }
    private static synchronized void installOnUiThread(Activity activity) {
        Window window=activity.getWindow();
        if(observed==window && window.getCallback()==wrapper && surface!=null && surface.isAttachedToWindow())return;
        detach();
        Window.Callback previous=window.getCallback();
        if(previous==null)return;
        final int token=++generation;
        Window.Callback observer=(Window.Callback)Proxy.newProxyInstance(
            PixelStickEvents.class.getClassLoader(),new Class<?>[]{Window.Callback.class},(proxy,method,args)->{
                if(method.getName().equals("onWindowFocusChanged"))focusChanged(token,(Boolean)args[0]);
                if(method.getName().equals("dispatchTouchEvent"))
                    record(window,token,(MotionEvent)args[0]);
                try{return method.invoke(previous,args);}
                catch(InvocationTargetException e){throw e.getCause();}
            });
        surface=findSurface(window.getDecorView());
        if(surface==null)return;
        surface.addOnAttachStateChangeListener(attachment);
        delegate=previous;wrapper=observer;observed=window;focused=activity.hasWindowFocus();
        window.setCallback(observer);
    }
    private static View findSurface(View view) {
        if(view instanceof SurfaceView)return view;
        if(view instanceof ViewGroup){
            ViewGroup group=(ViewGroup)view;
            for(int i=0;i<group.getChildCount();i++){
                View found=findSurface(group.getChildAt(i));
                if(found!=null)return found;
            }
        }
        return null;
    }
    public static void uninstall(Activity activity) {
        activity.runOnUiThread(()->{synchronized(PixelStickEvents.class){
            if(observed==activity.getWindow())detach();
        }});
    }
    private static synchronized void focusChanged(int token,boolean focus){
        if(token!=generation)return;
        focused=focus;if(!focus)disarm();
    }
    private static void detach() {
        disarm();generation++;
        if(surface!=null)surface.removeOnAttachStateChangeListener(attachment);
        if(observed!=null && observed.getCallback()==wrapper)observed.setCallback(delegate);
        observed=null;delegate=null;wrapper=null;surface=null;
    }
    public static synchronized void disarm() {
        armed=false;finger=-1;used=0;resetPending=true;
    }

    // Bounds are normalized to the actual Unity render surface, origin at the top left.
    // null means no observer installed, an empty array means an installed, idle stick.
    public static synchronized long[] poll(float l,float t,float r,float b) {
        return pending(l,t,r,b)<0?null:drain();
    }
    // Most rendered frames carry no stick event. Return a primitive in that
    // case so the JNI bridge never marshals an empty Java array into IL2CPP.
    public static synchronized int pending(float l,float t,float r,float b) {
        if(observed==null||surface==null||!focused)return -1;
        if(!armed||l!=left||t!=top||r!=right||b!=bottom){
            finger=-1;used=0;left=l;top=t;right=r;bottom=b;armed=r>l&&b>t;
            resetPending=true;
        }
        if(resetPending){append(0,CANCEL,0,0);resetPending=false;}
        return used;
    }
    public static synchronized long[] drain() {
        if(used==0)return EMPTY;
        long[] events=Arrays.copyOf(queue,used);used=0;return events;
    }
    private static void append(long time,int type,float x,float y) {
        if(used==queue.length){
            // A stalled scene may outlive the entire gesture. Break its command chain.
            used=0;append(time,CANCEL,0,0);
        }
        queue[used++]=time;queue[used++]=type;
        queue[used++]=Math.round(x*8192);queue[used++]=Math.round(y*8192);
    }
    private static float normalized(float coordinate,int size,float low,float high) {
        return (coordinate/size-low)*2f/(high-low)-1f;
    }
    private static synchronized void record(Window source,int token,MotionEvent event) {
        if(!armed||source!=observed||token!=generation||surface==null)return;
        int width=surface.getWidth(),height=surface.getHeight();
        if(width<=0||height<=0)return;
        int action=event.getActionMasked(),index=event.getActionIndex();
        if(action==MotionEvent.ACTION_CANCEL){
            if(finger!=-1)append(event.getEventTime(),CANCEL,0,0);
            finger=-1;return;
        }
        surface.getLocationOnScreen(surfaceOrigin);
        float dx=event.getRawX()-event.getX()-surfaceOrigin[0];
        float dy=event.getRawY()-event.getY()-surfaceOrigin[1];
        if(action==MotionEvent.ACTION_DOWN||action==MotionEvent.ACTION_POINTER_DOWN){
            if(finger!=-1 && action==MotionEvent.ACTION_DOWN){append(event.getEventTime(),CANCEL,0,0);finger=-1;}
            if(finger!=-1)return;
            float x=normalized(event.getX(index)+dx,width,left,right);
            float y=-normalized(event.getY(index)+dy,height,top,bottom);
            if(x*x+y*y<=1f){finger=event.getPointerId(index);append(event.getEventTime(),POINT,x,y);}
        }else if(action==MotionEvent.ACTION_MOVE && finger!=-1){
            int owner=event.findPointerIndex(finger);
            if(owner<0){append(event.getEventTime(),CANCEL,0,0);finger=-1;return;}
            for(int h=0;h<event.getHistorySize();h++)append(event.getHistoricalEventTime(h),POINT,
                normalized(event.getHistoricalX(owner,h)+dx,width,left,right),
                -normalized(event.getHistoricalY(owner,h)+dy,height,top,bottom));
            append(event.getEventTime(),POINT,normalized(event.getX(owner)+dx,width,left,right),
                -normalized(event.getY(owner)+dy,height,top,bottom));
        }else if((action==MotionEvent.ACTION_UP||action==MotionEvent.ACTION_POINTER_UP) && event.getPointerId(index)==finger){
            append(event.getEventTime(),LIFT,0,0);finger=-1;
        }
    }
}
