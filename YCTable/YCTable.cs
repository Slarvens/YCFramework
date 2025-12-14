using YCAnalyzer;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;
using YCTable.Handlers.Header;
namespace YCTable {
    public class YCTable {
        public static string value_rule_config = @"
 pair_list: pair ( ';' pair )*;
 pair: key_type '=' type_nonnull;
 key_type: basic_nonnull;
 // basic_nonnull are primitive tokens without nullable suffix
 basic_nonnull: @Integer | @Long | @Float | @Double | @String;
 // basic_type supports primitives and optional trailing '?' to indicate nullable
 basic_type: basic_nonnull ('?')?;
 // try composite forms first so tuple/array/dict are matched before single-token types; types can be nullable
 type_nonnull: ( tuple | array | dict | @Integer | @Long | @Float | @Double | @String | @Bool | @Identifier );
 type: type_nonnull ('?')?;
 // tuple items must be non-nullable basic types
 tuple: basic_nonnull ( '~' basic_nonnull )+;
 // element_type for arrays disallows nullable: element must be non-nullable basic/identifier/tuple/array
 element_type: basic_nonnull | @Bool | @Identifier | tuple | array;
 array: ( element_type ( ';' element_type )* )?;
 dict: pair_list?;
 ";

        // Grammar to parse table headers. start rule allows optional trailing '?'
        public static string header_rule_config = @"
 header_nullable: ( header_array | header_tuple | header_dict | header_primary ) ('?')?;
 // allow primary to carry optional '?' so parser accepts forms like Integer? inside groups
 header_primary: ( @Identifier | 'Integer' | 'Long' | 'Float' | 'Double' | 'String' | 'Bool' | 'json' ) ('?')?;
 header_array: header_primary '[' ']';
 header_tuple: '(' header_primary (',' header_primary)* ')';
 header_dict: 'dict' '<' header_primary ',' header_primary '>';
 ";

 
        // Helper: parse value into a simple ValueDescriptor (reuse token-based lightweight parsing)
        public abstract record ValueDescriptor;
        public record ValueBasic(string name) : ValueDescriptor;
        public record ValueArray(List<ValueDescriptor> elements) : ValueDescriptor;
        public record ValueTuple(List<ValueDescriptor> elements) : ValueDescriptor;
        public record ValueDict(List<(ValueDescriptor Key, ValueDescriptor Value)> Pairs) : ValueDescriptor;
        public record ValueNull() : ValueDescriptor;


        public static void register_header_handlers(AstProcessor ap, List<IHeader?> collector) {
            ap.register(new HeaderPrimaryHandler(collector));
            ap.register(new HeaderTupleHandler(collector));
            ap.register(new HeaderNullableHandler(collector));
            ap.register(new HeaderArrayHandler(collector));
            ap.register(new HeaderDictHandler(collector));
        }

        // Parse multiple header strings and return their TypeDescriptor representations.
        // If throwOnError is true, exceptions during parsing will be rethrown to the caller.
        public static List<IHeader?> parse_headers(string[] header_inputs, bool throwOnError = false) {
            if (header_inputs == null) {
                return new List<IHeader?>();
            }

            var results = new List<IHeader?>();
            var gp = new GrammarParser();
            var grammar = gp.parse(header_rule_config);
            var lexer = new Lexer();
            var ap = new AstProcessor();
            // single reusable collector passed to handlers; will be cleared per input
            var perCollector = new List<IHeader?>();
            register_header_handlers(ap, perCollector);

            foreach (var header in header_inputs) {
                var toks = lexer.parsing(header);
                var builder = new SyntaxBuilder(toks, grammar);
                var ast = builder.build();

                // clear previous results, handlers will append for this AST
                perCollector.Clear();
                ap.process(ast);

                // One header per input expected; use first produced or null
                results.Add(perCollector.Count >0 ? perCollector[0] : null);
            }

            return results;
        }
    }
}
