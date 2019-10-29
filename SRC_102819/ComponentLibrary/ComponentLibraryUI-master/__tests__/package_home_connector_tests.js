import {mapStateToProps} from '../src/package_home_connector';
import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('Package Home Connector', () => {
  describe('mapStateToProps', () => {
    it('should transform state and props',()=>{
      const state = {reducer:{packageGroupSearchDetails:{groups: ['hourly pkg', 'monthly pkg'], response: 'we found pkg in'}}};
      const props = {};
      const expectedOutput = {searchResponse:'we found pkg in', groups:['hourly pkg', 'monthly pkg']};
      const output = mapStateToProps(state,props);
      expect(output).to.deep.equal(expectedOutput);
    });
  });
});
