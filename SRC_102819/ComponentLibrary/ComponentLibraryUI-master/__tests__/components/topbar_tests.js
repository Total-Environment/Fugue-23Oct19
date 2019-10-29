import React from 'react';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import { shallow } from 'enzyme';
import { Topbar, __RewireAPI__ as TopbarRewired } from '../../src/components/topbar';

chai.use(chaiEnzyme());

describe('Topbar', () => {
  describe('render', () => {
    it('should have the proper classname', () => {
      TopbarRewired.__Rewire__('topbar', 'topbar')
      let wrapper = shallow(<Topbar className="sattar" />);
      expect(wrapper).to.have.className('topbar');
      expect(wrapper).to.have.className('sattar');
    })
  });
});
