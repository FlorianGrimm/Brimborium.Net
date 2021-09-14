namespace Brimborium.CodeFlow.FluentIL.Emitters
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using FluentIL;

    /// <summary>
    /// A debug implementation of the <see cref="IEmitter"/> interface to allow
    /// IL to be written to a <see cref="IDebugOutput"/> implemenation.
    /// </summary>
    internal class DebugEmitter
        : EmitterBase
    {
        /// <summary>
        /// The <see cref="IEmitter"/> to make the actual call to.
        /// </summary>
        private IEmitter emitter;

        /// <summary>
        /// The <see cref="IDebugOutput" /> to write output to.
        /// </summary>
        private IDebugOutput debugOutput;

        /// <summary>
        /// The current IL offset.
        /// </summary>
        private int offset;

        /// <summary>
        /// The current local index.
        /// </summary>
        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugEmitter"/> class.
        /// </summary>
        /// <param name="emitter">The emitter to call through to.</param>
        /// <param name="debugOutput">The debug output to write to.</param>
        public DebugEmitter(IEmitter emitter, IDebugOutput debugOutput)
        {
            this.emitter = emitter;
            this.debugOutput = debugOutput;
            this.offset = 0;
            this.index = 0;
        }

        /// <inheritdoc/>
        public override int ILOffset
        {
            get
            {
                return this.offset;
            }
        }

        /// <inheritdoc/>
        public override IEmitter BeginCatchBlock(Type exceptionType)
        {
            this.emitter?.BeginCatchBlock(exceptionType);

            this.OutputILOffset();
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tBeginCatchBlock({0})", exceptionType.Name);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter BeginExceptFilterBlock()
        {
            this.emitter?.BeginExceptFilterBlock();

            this.OutputILOffset();
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tBeginExceptFilterBlock");
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter BeginExceptionBlock(out ILabel label)
        {
            this.emitter?.BeginExceptionBlock(out label);

            this.OutputILOffset();
            label = new LabelAdapter(Guid.NewGuid().ToString());
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tBeginExceptionBlock [{0}]", label.Name);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter BeginExceptionBlock(ILabel label)
        {
            this.emitter?.BeginExceptionBlock(label);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter BeginFaultBlock()
        {
            this.emitter?.BeginFaultBlock();

            this.OutputILOffset();
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tBeginFaultBlock");
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter BeginFinallyBlock()
        {
            this.emitter?.BeginFinallyBlock();

            this.OutputILOffset();
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tBeginFinallyBlock");
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter BeginScope()
        {
            this.emitter?.BeginScope();

            this.OutputILOffset();
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tBeginScope");
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter DeclareLocal(Type localType, out ILocal local)
        {
            return this.DeclareLocal(localType, Guid.NewGuid().ToString(), false, out local);
        }

        /// <inheritdoc/>
        public override IEmitter DeclareLocal(Type localType, string localName, out ILocal local)
        {
            return this.DeclareLocal(localType, localName, false, out local);
        }

        /// <inheritdoc/>
        public override IEmitter DeclareLocal(Type localType, bool pinned, out ILocal local)
        {
            return this.DeclareLocal(localType, Guid.NewGuid().ToString(), pinned, out local);
        }

        /// <inheritdoc/>
        public override IEmitter DeclareLocal(Type localType, string localName, bool pinned, out ILocal local)
        {
            local = null;
            this.emitter?.DeclareLocal(localType, localName, out local);
            this.debugOutput.WriteLineColor(ConsoleColor.Yellow, "Local - [{0}] {1} ({2})", this.index++, localType.Name, localName);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter DeclareLocal(IGenericParameterBuilder genericParameter, out ILocal local)
        {
            local = null;
            this.emitter?.DeclareLocal(genericParameter, out local);
            this.debugOutput.WriteLineColor(ConsoleColor.Yellow, "Local - [{0}] Generic Type)", this.index++);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter DeclareLocal(Type localGenericTypeDefinition, IGenericParameterBuilder[] genericTypeArgs, out ILocal local)
        {
            local = null;
            this.emitter?.DeclareLocal(localGenericTypeDefinition, genericTypeArgs, out local);
            this.debugOutput.WriteLineColor(ConsoleColor.Yellow, "Local - [{0}] Generic Type {1}", this.index++, localGenericTypeDefinition.Name);
            return this;
        }

        /// <inheritdoc />
        public override IEmitter DeclareLocal(ITypeBuilder typeBuilder, out ILocal local)
        {
            local = null;
            this.emitter?.DeclareLocal(typeBuilder, out local);
            this.debugOutput.WriteLineColor(ConsoleColor.Yellow, "Local - [{0}] {1}", this.index++, typeBuilder.TypeName);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter DeclareLocal(ILocal local)
        {
            this.emitter?.DeclareLocal(local);
            this.debugOutput.WriteLineColor(ConsoleColor.Yellow, "Local - [{0}] {1}", this.index++, local.Name);
            return this;
        }

         /// <inheritdoc/>
        public override IEmitter DefineLabel(out ILabel label)
        {
            return this.DefineLabel(Guid.NewGuid().ToString(), out label);
        }

        /// <inheritdoc/>
        public override IEmitter DefineLabel(string labelName, out ILabel label)
        {
            label = null;
            this.emitter?.DefineLabel(labelName, out label);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter DefineLabel(ILabel label)
        {
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, Type type)
        {
            this.emitter?.Emit(opcode, type);

            this.OutputOpCode(opcode);

            if (type == null)
            {
                WriteLineError("\t!Null Type!");
                return this;
            }

            this.debugOutput.WriteLine("\t[{0}]", type.Name);

            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, string str)
        {
            this.emitter?.Emit(opcode, str);

            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t\"{0}\"", str ?? "<NULL>");
            if (str != null)
            {
                this.offset += str.Length;
            }

            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, float arg)
        {
            this.emitter?.Emit(opcode, arg);

            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("    {0}\t{1}", opcode, arg);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, sbyte arg)
        {
            this.emitter?.Emit(opcode, arg);

            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t{0}", arg);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, MethodInfo methodInfo)
        {
            this.emitter?.Emit(opcode, methodInfo);

            this.OutputOpCode(opcode);

            if (methodInfo == null)
            {
                WriteLineError("\t!Null MethodInfo!");
                return this;
            }

            this.debugOutput.WriteLine("\t[{0}] {1}.{2}()", methodInfo.ReturnType, methodInfo.DeclaringType, methodInfo.Name);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, FieldInfo field)
        {
            this.emitter?.Emit(opcode, field);

            this.OutputOpCode(opcode);

            if (field == null)
            {
                WriteLineError("\t!Null FieldInfo!");
                return this;
            }

            this.debugOutput.WriteLine("\t[{0}] {1}.{2}", field.FieldType, field.DeclaringType, field.Name);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, IFieldBuilder field)
        {
            return this.Emit(opcode, field.Define());
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, ILabel[] labels)
        {
            this.emitter?.Emit(opcode, labels);

            this.OutputOpCode(opcode);

            if (labels == null)
            {
                WriteLineError("\t!Null ILabel[]!");
                return this;
            }

            this.debugOutput.WriteLine("\t[{0}]", string.Join(", ", labels.Select(l => l.Name)));
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, SignatureHelper signature)
        {
            this.emitter?.Emit(opcode, signature);

            this.OutputOpCode(opcode);

            if (signature == null)
            {
                WriteLineError("\t!Null SignatureHelper!");
                return this;
            }

            this.debugOutput.WriteLine("\t[{0}]", opcode, signature);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, ILocal local)
        {
            local = local ?? throw new ArgumentNullException(nameof(local));

            this.emitter?.Emit(opcode, local);

            int localIndex = local.LocalIndex;

            if (opcode.Equals(OpCodes.Ldloc))
            {
                switch (localIndex)
                {
                    case 0:
                        opcode = OpCodes.Ldloc_0;
                        break;

                    case 1:
                        opcode = OpCodes.Ldloc_1;
                        break;

                    case 2:
                        opcode = OpCodes.Ldloc_2;
                        break;

                    case 3:
                        opcode = OpCodes.Ldloc_3;
                        break;

                    default:
                        if (localIndex <= 255)
                        {
                            opcode = OpCodes.Ldloc_S;
                        }

                        break;
                }
            }
            else if (opcode.Equals(OpCodes.Stloc))
            {
                switch (localIndex)
                {
                    case 0:
                        opcode = OpCodes.Stloc_0;
                        break;

                    case 1:
                        opcode = OpCodes.Stloc_1;
                        break;

                    case 2:
                        opcode = OpCodes.Stloc_2;
                        break;

                    case 3:
                        opcode = OpCodes.Stloc_3;
                        break;

                    default:
                        if (localIndex <= 255)
                        {
                            opcode = OpCodes.Stloc_S;
                        }

                        break;
                }
            }
            else if (opcode.Equals(OpCodes.Ldloca) && localIndex <= 255)
            {
                opcode = OpCodes.Ldloca_S;
            }

            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t[{0}]", local.Name);
            this.offset += OpCodes.TakesSingleByteArgument(opcode) ? 1 : 2;
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, ConstructorInfo con)
        {
            this.emitter?.Emit(opcode, con);
            this.OutputOpCode(opcode);

            if (con == null)
            {
                WriteLineError("\t!Null ConstructorInfo!");
                return this;
            }

            this.debugOutput.WriteLine("\t{0}({1})", con.Name, string.Join(", ", con.GetParameters().Select(p => $"{p.ParameterType} {p.Name}")));
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, long arg)
        {
            this.emitter?.Emit(opcode, arg);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t{0}", arg);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, int arg)
        {
            this.emitter?.Emit(opcode, arg);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t{0}", arg);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, short arg)
        {
            this.emitter?.Emit(opcode, arg);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t{0}", arg);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, double arg)
        {
            this.emitter?.Emit(opcode, arg);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t{0}", arg);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, byte arg)
        {
            this.emitter?.Emit(opcode, arg);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t{0}", arg);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode)
        {
            this.emitter?.Emit(opcode);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine(string.Empty);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Emit(OpCode opcode, ILabel label)
        {
            this.emitter?.Emit(opcode, label);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLineColor(ConsoleColor.Cyan, "\t[{0}]", label.Name);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EmitCall(OpCode opcode, Func<MethodInfo> action)
        {
            this.emitter?.EmitCall(
                opcode,
                () =>
                {
                    var methodInfo = action();
                    this.OutputOpCode(opcode);
                    if (methodInfo == null)
                    {
                        this.debugOutput.WriteLine("\tNull Pointer");
                    }
                    else
                    {
                        this.debugOutput.WriteLine("\t{0})", methodInfo.Name);
                    }

                    return methodInfo;
                });

            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            this.emitter?.EmitCall(opcode, methodInfo, optionalParameterTypes);
            this.OutputOpCode(opcode);
            if (methodInfo == null)
            {
                this.debugOutput.WriteLine("    {0}\tNull Pointer", opcode);
                return this;
            }

            this.debugOutput.WriteLine("\t{0}({1})", methodInfo.Name, string.Join(", ", optionalParameterTypes.Select(t => t.Name)));
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
        {
            this.emitter?.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
            this.OutputOpCode(opcode);
            this.debugOutput.WriteLine("\t{0} {1}({2}) {3}", callingConvention, returnType.Name, string.Join(", ", parameterTypes.Select(t => t.Name)), string.Join(", ", optionalParameterTypes.Select(t => t.Name)));
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EmitWriteLine(FieldInfo fld)
        {
            if (fld == null)
            {
                WriteLineError("\t!Null FieldInfo!");
                return this;
            }

            MethodInfo getConsoleOut = typeof(Console)
                .GetProperty("Out")
                .GetGetMethod();

            this.Call(getConsoleOut);
            if ((fld.Attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope)
            {
                this.Emit(OpCodes.Ldsfld, fld);
            }
            else
            {
                this.LdArg0();
                this.LdFld(fld);
            }

            Type[] array = new Type[1];
            object fieldType = fld.FieldType;
            if (fieldType is TypeBuilder || fieldType is EnumBuilder)
            {
                throw new NotSupportedException("NotSupported_OutputStreamUsingTypeBuilder");
            }

            MethodInfo textWriterWriteLine = typeof(TextWriter)
                .GetMethod("WriteLine", new[] { fld.FieldType });

            if (textWriterWriteLine == null)
            {
                throw new ArgumentException("Argument_EmitWriteLineType", "fld");
            }

            this.CallVirt(textWriterWriteLine);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EmitWriteLine(string value)
        {
            this.LdStr(value);
            MethodInfo method = typeof(Console)
                .GetMethod("WriteLine", new Type[] { typeof(string) });
            this.Call(method);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EmitWriteLine(ILocal local)
        {
            this.debugOutput.WriteLineColor(
                ConsoleColor.DarkYellow,
                "-----EmitWriteLine-Start----");

            MethodInfo method = typeof(Console)
                .GetProperty("Out")
                .GetGetMethod();

            this.Call(method);
            this.LdLoc(local);
            Type[] array = new Type[1];
            object localType = local.LocalType;
            if (localType is TypeBuilder ||
                localType is EnumBuilder)
            {
                throw new ArgumentException("NotSupported_OutputStreamUsingTypeBuilder");
            }

            array[0] = (Type)localType;
            MethodInfo textWriterWriteLine = typeof(TextWriter)
                .GetMethod("WriteLine", array);

            if (textWriterWriteLine == null)
            {
                throw new ArgumentException("Argument_EmitWriteLineType", "localBuilder");
            }

            this.CallVirt(textWriterWriteLine);
            this.debugOutput.WriteLineColor(
                ConsoleColor.DarkYellow,
                "-----EmitWriteLine-End-----");

            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EndExceptionBlock()
        {
            this.emitter?.EndExceptionBlock();
            this.OutputILOffset();
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tEndExceptionBlock");
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter EndScope()
        {
            this.emitter?.EndScope();
            this.OutputILOffset();
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "\tEndScope");
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter MarkLabel(ILabel label)
        {
            this.emitter?.MarkLabel(label);
            this.debugOutput.WriteLineColor(ConsoleColor.Cyan, "[{0}]", label.Name);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter ThrowException(Type excType)
        {
            this.emitter?.ThrowException(excType);
            this.OutputILOffset();
            this.debugOutput.WriteLine("\tThrow Excpetion ({0})", excType.Name);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter UsingNamespace(string @namespace)
        {
            this.emitter?.UsingNamespace(@namespace);
            this.OutputILOffset();
            this.debugOutput.WriteLine("\t using {0}", @namespace);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Comment(string comment)
        {
            this.debugOutput.WriteLineColor(ConsoleColor.Green, "// {0}", comment);
            return this;
        }

        /// <inheritdoc/>
        public override IEmitter Defer(Action<IEmitter> action)
        {
            action(this);
            return this;
        }

        /// <inheritdoc/>
        public override void EmitIL(ILGenerator generator)
        {
            ((EmitterBase)this.emitter)?.EmitIL(generator);
        }

        /// <summary>
        /// Output the IL offset.
        /// </summary>
        private void OutputILOffset()
        {
            this.debugOutput.Write(this.offset.ToString("0x0000000"));
            this.offset = this.emitter?.ILOffset ?? 0;
        }

        /// <summary>
        /// Outputs the <see cref="OpCode"/>.
        /// </summary>
        /// <param name="opcode">An OpCode.</param>
        private void OutputOpCode(OpCode opcode)
        {
            this.OutputILOffset();
            this.debugOutput.Write("\t{0}", opcode);
        }
    }
}