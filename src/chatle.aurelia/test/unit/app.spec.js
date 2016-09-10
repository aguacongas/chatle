define(["require", "exports", '../../src/app'], function (require, exports, app_1) {
    "use strict";
    describe('the app', function () {
        it('says hello', function () {
            expect(new app_1.App().message).toBe('Hello World!');
        });
    });
});
//# sourceMappingURL=app.spec.js.map