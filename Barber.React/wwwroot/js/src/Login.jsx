var LoginPage = React.createClass({
    displayName: 'Login',
    render: function () {
        return (
            React.createElement('div', { className: "container" },
                "Hello, world! I am a CommentBox."
            )
        );
    }
});
ReactDOM.render(
    React.createElement(LoginPage, null),
    document.getElementById('content')
);