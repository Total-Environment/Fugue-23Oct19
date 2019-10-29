import React from 'react';
import { shallow } from 'enzyme';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import { Icon } from '../../src/components/icon';

chai.use(chaiEnzyme());

describe('Icon', () => {
  describe('render', () => {
    it('should render the proper icon with path', () => {
      const wrapper = shallow(<Icon name="materials" className="materials-icon"/>);
      expect(wrapper).to.have.descendants('path');
      expect(wrapper).to.have.className('materials-icon');
    });
  });
});
