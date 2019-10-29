import {shallow} from 'enzyme';
import React from 'react';
import {SidebarActual, __RewireAPI__ as SidebarRewired} from '../../src/components/sidebar';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('Sidebar', () => {
  describe('render', () => {
    it('should have proper class name', () => {
      SidebarRewired.__Rewire__('sidebar', 'sidebar');
      SidebarRewired.__Rewire__('collapsed', 'collapsed');
      const wrapper = shallow(<SidebarActual className="extraclass" />);
      expect(wrapper).to.have.className('sidebar');
      expect(wrapper).to.have.className('extraclass');
      expect(wrapper).to.not.have.className('collapsed');
    });

    it('should have label when collapsed is false', () => {
      const wrapper = shallow(<SidebarActual collapsed={false}/>);
      expect(wrapper).to.contain(' Materials');
      expect(wrapper.find('Link').first()).to.have.descendants('Icon');
    });

    it('should not label have when collapsed is true', () => {
      SidebarRewired.__Rewire__('link', 'link');
      const wrapper = shallow(<SidebarActual collapsed />);
      const link = wrapper.find('Link.link').first();
      expect(link).to.have.descendants('Icon');
      expect(link.prop('children')).to.have.length(2);
      expect(link.prop('children')[1]).to.equal('');
    });

    it('should have class name collapsed when it is collapsed', () => {
      SidebarRewired.__Rewire__('collapsed', 'collapsed');
      const wrapper = shallow(<SidebarActual collapsed />);
      expect(wrapper).to.have.className('collapsed');
    });

    it('on mouse enter it should not have the class name collapsed', () => {
      const wrapper = shallow(<SidebarActual collapsed/>);
      wrapper.prop('onMouseEnter')();
      expect(wrapper.update()).to.not.have.className('collapsed');
    });

    it('on mouse leave it should have the class name collapsed', () => {
      const wrapper = shallow(<SidebarActual />);
      wrapper.prop('onMouseLeave')();
      expect(wrapper.update()).to.have.className('collapsed');
    });
  });
});
