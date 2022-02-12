using BepInEx;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BetterAPI
{
    public abstract class BetterUnityPlugin<T> : BaseUnityPlugin
    {
        public abstract BaseUnityPlugin typeReference { get; }

        public static HookManager Hooks = new HookManager();
        public static event Action onAwake;
        public static event Action onStart;
        public static event Action onEnable;
        public static event Action onDisable;
        public static event Action onUpdate;
        public static event Action onFixedUpdate;
        public static event Action onLateUpdate;

        virtual protected void Awake()
        {
            if (onAwake != null) onAwake();
        }

        virtual protected void Start()
        {
            if (onStart != null) onStart();
        }
        virtual protected void OnEnable()
        {
            if (onEnable != null) onEnable();
        }
        virtual protected void OnDisable()
        {
            if (onDisable != null) onDisable();
        }
        virtual protected void Update()
        {
            if (onUpdate != null) onUpdate();
        }
        virtual protected void FixedUpdate()
        {
            if (onFixedUpdate != null) onFixedUpdate();
        }
        virtual protected void LateUpdate()
        {
            if (onLateUpdate != null) onLateUpdate();
        }

        public class HookManager
        { 
            private List<(MethodInfo methodFrom, MethodInfo methodTo)> hookSignatures = new List<(MethodInfo, MethodInfo)>();
            private List<(MethodInfo methodFrom, MonoMod.Cil.ILContext.Manipulator ILHookMethod)> ILHookMethods = new List<(MethodInfo, MonoMod.Cil.ILContext.Manipulator)>();
            private List<Hook> hooks = new List<Hook>();
            private List<ILHook> ILHooks = new List<ILHook>();
            private bool enabled = false;

            public HookManager()
            {
                onEnable += BetterUnityPlugin_onEnable;
                onDisable += BetterUnityPlugin_onDisable;
            }

            private void BetterUnityPlugin_onEnable()
            {
                enabled = true;
                foreach (var hook in hookSignatures)
                {
                    hooks.Add(new Hook(hook.methodFrom, hook.methodTo));
                }
                foreach (var hook in ILHookMethods)
                {
                    ILHooks.Add(new ILHook(hook.methodFrom, hook.ILHookMethod));
                }
            }

            private void BetterUnityPlugin_onDisable()
            {
                enabled = false;
                foreach (var hook in hooks)
                {
                    hook.Dispose();
                }
                foreach (var hook in ILHooks)
                {
                    hook.Dispose();
                }
            }


            public static MethodInfo FindMethod(Type type, string methodName, Type[] types, BindingFlags bindings = BindingFlags.Default)
            {
                MethodInfo methodFrom;
                if (bindings != BindingFlags.Default)
                {
                    methodFrom = type.GetMethod(methodName, bindings, null, types, null)
                        ?? type.GetMethod(methodName, bindings);
                }
                else
                {
                    methodFrom = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, types, null)
                        ?? type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, types, null)
                        ?? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, types, null)
                        ?? type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static, null, types, null)
                        ?? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                        ?? type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)
                        ?? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
                        ?? type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
                }
                return methodFrom;
            }
            public static Type[] getParameterTypes(MethodInfo methodInfo) 
            {
                List<Type> types = new List<Type>();
                var parameters = methodInfo.GetParameters();
                for (int i = 1; i < parameters.Length; i++)
                {
                    types.Add(parameters[i].ParameterType);
                }
                return types.ToArray();
            }
            public void Add(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { }, ILHookMethod, bindings); }
            public void Add(Type type, string methodName, Delegate hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { }, hookMethod.Method, bindings); }
            public void Add(Type type, string methodName, Action<Action> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { }, hookMethod.Method, bindings); }
            public void Add<T1>(string methodName, Delegate hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, getParameterTypes(hookMethod.Method), hookMethod.Method, bindings); }
            public void Add<T1>(Type type, string methodName, Action<Action<T1>, T1> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1) }, hookMethod.Method, bindings); }
            public void Add<T1>(Type type, string methodName, Func<Func<T1>, T1> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { }, hookMethod.Method, bindings); }
            public void Add<T1>(string methodName, Action<Action> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { }, hookMethod.Method, bindings); }
            public void Add<T1>(string methodName, Action<Action<T1>, T1> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { }, hookMethod.Method, bindings); }
            public void Add<T1>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] {  }, ILHookMethod, bindings); }
            public void Add<T1>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1) }, ILHookMethod, bindings); }
            public void Add<T1, T2>(Type type, string methodName, Action<Action<T1, T2>, T1, T2> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2) }, hookMethod.Method, bindings); }
            public void Add<T1, T2>(Type type, string methodName, Func<Func<T1, T2>, T1, T2> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), }, hookMethod.Method, bindings); }
            public void Add<T1, T2>(string methodName, Action<Action<T2>, T2> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2) }, hookMethod.Method, bindings); }
            public void Add<T1, T2>(string methodName, Func<Func<T2>, T2> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { }, hookMethod.Method, bindings); }
            public void Add<T1, T2>(string methodName, Action<Action<T1, T2>, T1, T2> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2) }, hookMethod.Method, bindings); }
            public void Add<T1, T2>(string methodName, Func<Func<T1, T2>, T1, T2> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { }, hookMethod.Method, bindings); }
            public void Add<T1, T2>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2) }, ILHookMethod, bindings); }
            public void Add<T1, T2>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3>(Type type, string methodName, Action<Action<T1, T2, T3>, T1, T2, T3> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3>(Type type, string methodName, Func<Func<T1, T2, T3>, T1, T2, T3> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3>(string methodName, Action<Action<T2, T3>, T2, T3> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3>(string methodName, Func<Func<T2, T3>, T2, T3> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3>(string methodName, Action<Action<T1, T2, T3>, T1, T2, T3> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3>(string methodName, Func<Func<T1, T2, T3>, T1, T2, T3> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4>(Type type, string methodName, Action<Action<T1, T2, T3, T4>, T1, T2, T3, T4> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4>(Type type, string methodName, Func<Func<T1, T2, T3, T4>, T1, T2, T3, T4> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4>(string methodName, Action<Action<T2, T3, T4>, T2, T3, T4> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4>(string methodName, Func<Func<T2, T3, T4>, T2, T3, T4> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4>(string methodName, Action<Action<T1, T2, T3, T4>, T1, T2, T3, T4> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4>(string methodName, Func<Func<T1, T2, T3, T4>, T1, T2, T3, T4> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5>(string methodName, Action<Action<T2, T3, T4, T5>, T2, T3, T4, T5> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5>(string methodName, Func<Func<T2, T3, T4, T5>, T2, T3, T4, T5> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5>(string methodName, Action<Action<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5>(string methodName, Func<Func<T1, T2, T3, T4, T5>, T1, T2, T3, T4, T5> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6>, T1, T2, T3, T4, T5, T6> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6>, T1, T2, T3, T4, T5, T6> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(string methodName, Action<Action<T2, T3, T4, T5, T6>, T2, T3, T4, T5, T6> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(string methodName, Func<Func<T2, T3, T4, T5, T6>, T2, T3, T4, T5, T6> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6>, T1, T2, T3, T4, T5, T6> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6>, T1, T2, T3, T4, T5, T6> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7>, T1, T2, T3, T4, T5, T6, T7> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7>, T1, T2, T3, T4, T5, T6, T7> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7>, T2, T3, T4, T5, T6, T7> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7>, T2, T3, T4, T5, T6, T7> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7>, T1, T2, T3, T4, T5, T6, T7> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7>, T1, T2, T3, T4, T5, T6, T7> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8>, T1, T2, T3, T4, T5, T6, T7, T8> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8>, T1, T2, T3, T4, T5, T6, T7, T8> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8>, T2, T3, T4, T5, T6, T7, T8> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8>, T2, T3, T4, T5, T6, T7, T8> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8>, T1, T2, T3, T4, T5, T6, T7, T8> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8>, T1, T2, T3, T4, T5, T6, T7, T8> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>, T1, T2, T3, T4, T5, T6, T7, T8, T9> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9>, T1, T2, T3, T4, T5, T6, T7, T8, T9> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8, T9>, T2, T3, T4, T5, T6, T7, T8, T9> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8, T9>, T2, T3, T4, T5, T6, T7, T8, T9> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>, T1, T2, T3, T4, T5, T6, T7, T8, T9> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9>, T1, T2, T3, T4, T5, T6, T7, T8, T9> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8, T9, T10>, T2, T3, T4, T5, T6, T7, T8, T9, T10> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10>, T2, T3, T4, T5, T6, T7, T8, T9, T10> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Type type, string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Type type, string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string methodName, Action<Action<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string methodName, Func<Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string methodName, Action<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15) }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string methodName, Func<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> hookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), }, hookMethod.Method, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(typeof(T1), methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15) }, ILHookMethod, bindings); }
            public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Type type, string methodName, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default) { Add(type, methodName, new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15) }, ILHookMethod, bindings); }

            public void Add(Type type, string methodName, Type[] types, MonoMod.Cil.ILContext.Manipulator ILHookMethod, BindingFlags bindings = BindingFlags.Default)
            {
                MethodInfo methodFrom = FindMethod(type, methodName, types, bindings);
                if (methodFrom == null)
                {
                    UnityEngine.Debug.LogError($"Could not hook method {methodName} of {type}, method not found.");
                    return;
                }
                ILHookMethods.Add((methodFrom, ILHookMethod));
                if (enabled) ILHooks.Add(new ILHook(methodFrom, ILHookMethod));
            }
            public void Add(Type type, string methodName, Type[] types, MethodInfo hookMethod, BindingFlags bindings = BindingFlags.Default)
            {

                MethodInfo methodFrom = FindMethod(type, methodName, types, bindings);
                if (methodFrom == null)
                {
                    UnityEngine.Debug.LogError($"Could not hook method {methodName} of {type}, method not found.");
                    return;
                }
                hookSignatures.Add((methodFrom, hookMethod));
                if (enabled) hooks.Add(new Hook(methodFrom, hookMethod));
            }
        }
    }
}