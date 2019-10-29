import {mapStateToProps} from '../src/service_home_connector';
import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('Service Home Connector', () => {
    describe('mapStateToProps', () => {
        it('should transform state and props',()=>{
            const state = {reducer:{serviceGroupSearchDetails:{groups: ['Clay services', 'Cement services'], response: 'we found teakwood in'}}};
            const props = {};
            const expectedOutput = {searchResponse:'we found teakwood in', groups:['Clay services', 'Cement services']};
            const output = mapStateToProps(state,props);
            expect(output).to.deep.equal(expectedOutput);
        });
    });
});
