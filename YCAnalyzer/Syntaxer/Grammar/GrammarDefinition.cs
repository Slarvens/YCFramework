namespace YCAnalyzer.Syntaxer.Grammar {
    // 所有语法项的基类
    public abstract record GrammarItem;

    // 表示对另一个规则的引用
    public record RuleRef(string Name) : GrammarItem;

    // 表示一个字面量 Token，例如 'dict' 或 '<'
    public record Literal(string Lexeme, TokenType Type) : GrammarItem;

    // 表示一个 Token 类型，例如 @Identifier
    public record TokenTypeRef(TokenType Type) : GrammarItem;

    // 表示一个可选部分，例如 (...)`?`
    public record Optional(GrammarItem Item) : GrammarItem;

    // 表示重复（零次或多次），例如 (...)*
    public record Repetition(GrammarItem Item) : GrammarItem;

    // 表示至少一次的重复，例如 (...)+
    public record OneOrMore(GrammarItem Item) : GrammarItem;

    // 通用量词，表示 {min,max}，max 为 null 表示无限
    public record Quantifier(GrammarItem Item, int Min, int? Max) : GrammarItem;

    // 表示一个用分隔符分隔的列表，例如 a (sep a)*
    public record Separated(GrammarItem Item, GrammarItem Separator) : GrammarItem;

    // 表示一个序列，即一系列必须按顺序匹配的语法项
    public record Sequence(params GrammarItem[] Items) : GrammarItem;

    // 表示一个选项，即必须匹配其中之一的一系列语法项
    public record Choice(params GrammarItem[] Choices) : GrammarItem;

    /// <summary>
    /// 否定先行断言（不消费输入），相当于 PEG 的 !predicate
    /// </summary>
    public record NotPredicate(GrammarItem Item) : GrammarItem;

    /// <summary>
    /// 肯定先行断言（不消费输入），相当于 PEG 的 &predicate
    /// </summary>
    public record AndPredicate(GrammarItem Item) : GrammarItem;

    /// <summary>
    /// 匹配任意单个 token（通配符）
    /// </summary>
    public record Any : GrammarItem;

    /// <summary>
    /// 给子项命名或附加元数据，便于后续的 AST处理
    /// </summary>
    public record Named(GrammarItem Item, string Name) : GrammarItem;

    /// <summary>
    /// Represents an empty production (epsilon), used for grammar transformation.
    /// </summary>
    public record Epsilon : GrammarItem;

    /// <summary>
    /// 存储整个语言的语法定义。
    /// </summary>
    public class GrammarDefinition {
        /// <summary>
        /// The starting rule of the grammar.
        /// </summary>
        public string start_rule { get; }

        private readonly Dictionary<string, GrammarItem> _rules;

        /// <summary>
        /// Provides read-only access to all rules in the grammar.
        /// </summary>
        public IReadOnlyDictionary<string, GrammarItem> AllRules => _rules;

        public GrammarDefinition(string startRule, Dictionary<string, GrammarItem> rules) {
            start_rule = startRule;
            _rules = rules;
        }

        /// <summary>
        /// Gets a specific grammar rule by its name.
        /// </summary>
        /// <param name="name">The name of the rule.</param>
        /// <returns>The grammar item for the rule.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the rule is not found.</exception>
        public GrammarItem get_rule(string name) {
            if (_rules.TryGetValue(name, out var rule)) {
                return rule;
            }
            throw new KeyNotFoundException($"Grammar rule '{name}' not found.");
        }
    }
}
