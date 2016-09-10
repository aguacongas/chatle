define(["require", "exports", 'karma', 'aurelia-cli'], function (require, exports, karma_1, aurelia_cli_1) {
    "use strict";
    function unit(done) {
        new karma_1.Server({
            configFile: __dirname + '/../../karma.conf.js',
            singleRun: !aurelia_cli_1.CLIOptions.hasFlag('watch')
        }, done).start();
    }
    exports.unit = unit;
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = unit;
});
//# sourceMappingURL=test.js.map