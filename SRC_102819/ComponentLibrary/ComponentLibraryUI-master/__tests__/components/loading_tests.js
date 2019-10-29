import React from 'react';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import { shallow } from 'enzyme';
import { Loading } from '../../src/components/loading';

chai.use(chaiEnzyme());

describe('Loading', () => {
  describe('render', () => {
    it('should render icon', () => {
      const wrapper = shallow(<Loading />);
      expect(wrapper).to.have.descendants('Icon');
    });
  });
});
